using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

namespace Utilities.Concurrency
{
    /// <summary>
    /// ParallelWork allows you to run operations in separate thread yet receive
    /// success, failure and progress update on the WPF UI thread so that you can
    /// have a responsive UI and carry out expensive operations in background.
    /// 
    /// It's convenient to use than BackgroundWorker component. No need to decare 
    /// events and preserve stuffs in private variables to access it from different
    /// event callbacks. You can return data from parallel thread to the success/fail
    /// callback safely in a strongly typed manner.
    /// </summary>
    public static class ParallelWork
    {
        private static readonly List<Thread> _threadPool = new List<Thread>();
        private static readonly List<DispatcherTimer> _timerPool = new List<DispatcherTimer>();

        private static ManualResetEvent _AllBackgroundThreadCompletedEvent = new ManualResetEvent(true);
        private static ManualResetEvent _AllTimerFiredEvent = new ManualResetEvent(true);

        public static void StartNow(Action doWork, Action onComplete)
        {
            StartNow(doWork, onComplete, (x) => { throw x; });
        }

        public static void StartNow(Action doWork, Action onComplete, Action<Exception> failed)
        {
            StartNow<object>(() => { doWork(); return true; }, (o) => onComplete(), failed);
        }

        public static void StartNow<T>(Func<T> doWork, Action<T> onComplete)
        {
            StartNow<T>(doWork, onComplete, (x) => { throw x; });
        }

        public static void StartNow<T>(Func<T> doWork, Action<T> onComplete, Action<Exception> fail)
        {
            StartNow<object, T>(new object(), 
                (o, progressCallback) => { return doWork(); },
                (o, msg, done) => { },
                (o, result) => onComplete(result),
                (o, x) => { fail(x); }
                );
        }

        public static void StartNow<T, R>(
            T arg,
            Func<T, Action<T, string, int>, R> doWork,
            Action<T, string, int> progress,
            Action<T, R> onComplete,
            Action<T, Exception> fail)
        {
            Weak<Dispatcher> currentDispatcher = Dispatcher.CurrentDispatcher;
            Thread newThread = new Thread(new ParameterizedThreadStart( (thread)=>
                {
                    var currentThread = thread as Thread;

                    Dispatcher dispatcher = currentDispatcher;
                    if (null == dispatcher)
                        fail(arg, new ApplicationException("Dispatcher is unavailable"));
                        
                    try
                    {
                        Debug.WriteLine(currentThread.ManagedThreadId + " Work execution stated: " + DateTime.Now.ToString());
                        
                        R result = doWork(arg,
                            (data, message, percent) => dispatcher.BeginInvoke(progress, arg, message, percent));

                        if (null == result)
                        {
                            try
                            {
                                dispatcher.BeginInvoke(fail, arg, null);
                            }
                            catch
                            {
                                // Incase the error handler produces exception, we have to gracefully
                                // handle it since this is a background thread
                            }
                            finally
                            {
                                // Nothing to do, error handler is not supposed to produce more error
                            }
                        }
                        else
                        {
                            try
                            {
                                dispatcher.BeginInvoke(onComplete, arg, result);
                            }
                            catch (Exception x)
                            {
                                dispatcher.BeginInvoke(fail, arg, x);
                            }
                        }
                    }
                    catch (ThreadAbortException ex)
                    {
                        Debug.WriteLine(ex);
                    }
                    catch (Exception x)
                    {
                        dispatcher.BeginInvoke(fail, arg, x);
                    }
                    finally
                    {
                        currentDispatcher.Dispose();
                        Debug.WriteLine(currentThread.ManagedThreadId + " Work execution completed: " + DateTime.Now.ToString());

                        lock (_threadPool)
                        {
                            _threadPool.Remove(thread as Thread);
                            if (_threadPool.Count == 0)
                            {
                                _AllBackgroundThreadCompletedEvent.Set();
                                Debug.WriteLine("All Work completed: " + DateTime.Now.ToString());
                            }
                        }
                    }
                }));
            
            // Store the thread in a pool so that it is not garbage collected
            lock(_threadPool) 
                _threadPool.Add(newThread);

            _AllBackgroundThreadCompletedEvent.Reset();
            Debug.WriteLine(newThread.ManagedThreadId + " Work queued at: " + DateTime.Now.ToString());            

            newThread.SetApartmentState(ApartmentState.STA);
            newThread.Start(newThread);            
        }

        public static DispatcherTimer StartAfter(
            Action onComplete,
            TimeSpan duration)
        {
            return StartAfter(() => { }, (msg, done) => { }, onComplete, (x) => { throw x; }, duration);
        }

        public static DispatcherTimer StartAfter(
            Action doWork,
            Action onComplete,
            TimeSpan duration)            
        {
            return StartAfter(doWork, (msg, done) => { }, onComplete, (x) => { throw x; }, duration);
        }

        public static DispatcherTimer StartAfter(
            Action doWork,
            Action onComplete,
            Action<Exception> onException,
            TimeSpan duration)
        {
            return StartAfter(doWork, (msg, done) => { }, onComplete, onException, duration);
        }
        
        public static DispatcherTimer StartAfter(
            Action doWork, 
            Action<string, int> onProgress,
            Action onComplete, 
            Action<Exception> onError, 
            TimeSpan duration)
        {
            var currentDispatcher = Dispatcher.CurrentDispatcher;
            return StartAfter<Dispatcher, bool>(currentDispatcher,
                (dispatcher, progress) => { doWork(); return true; }, 
                (dispatcher, msg, done) => onProgress(msg, done), 
                (dispatcher, result) => onComplete(),
                (dispatcher, x) => onError(x), 
                duration);
        }

        public static DispatcherTimer StartAfter<T, R>(
            T arg,
            Func<T, Action<T, string, int>, R> doWork, 
            Action<T, string, int> onProgress,
            Action<T, R> onComplete, 
            Action<T, Exception> onError, 
            TimeSpan duration)
        {
            var timer = new DispatcherTimer(duration, DispatcherPriority.Normal, new EventHandler((sender, e) =>
            {
                var currentTimer = (sender as DispatcherTimer);
                currentTimer.Stop();
                lock (_timerPool)
                {
                    _timerPool.Remove(currentTimer);
                    if (_timerPool.Count == 0)
                        _AllTimerFiredEvent.Set();
                }

                ParallelWork.StartNow<T, R>(arg, doWork, onProgress, onComplete, onError);
            }),
            Dispatcher.CurrentDispatcher);

            _AllTimerFiredEvent.Reset();
            
            lock(_timerPool)
                _timerPool.Add(timer);
            timer.Start();

            return timer;
        }

        public static void StopAll()
        {
            while (_timerPool.Count > 0)
            {
                DispatcherTimer timer = _timerPool[0];
                try
                {
                    timer.Stop();
                }
                finally
                {
                    lock (_timerPool)
                        if (_timerPool.Contains(timer))
                            _timerPool.Remove(timer);
                }
            }

            while (_threadPool.Count > 0)
            {
                Thread t = _threadPool[0];
                try
                {
                    t.Abort();
                }
                finally
                {
                    lock (_threadPool)
                        if (_threadPool.Contains(t))
                            _threadPool.Remove(t);
                }
            }            
        }

        public static bool IsWorkOrTimerQueued()
        {
            lock (_timerPool)
                if (_timerPool.Count > 0)
                    return true;
            lock (_threadPool)
                if (_threadPool.Count > 0)
                    return true;

            return false;
        }

        public static bool IsAnyWorkRunning()
        {
            lock (_threadPool)
                if (_threadPool.Count > 0)
                    return true;
            return false;
        }

        public static bool WaitForAllWork(TimeSpan timeout)
        {
            lock (_threadPool)
                if (_threadPool.Count == 0)
                    return true;

            Debug.WriteLine("Start waiting: " + DateTime.Now.ToString());
            var result = _AllBackgroundThreadCompletedEvent.WaitOne(timeout);
            Debug.WriteLine("End waiting: " + DateTime.Now.ToString());
            return result;
        }
    }

    public class Start
    {
        private Func<Action<object, string, int>, object> workHandlerWithProgress;
        private Action successHandler = () => { };
        private Action<Exception> exceptionHandler = (x) => { };
        private Action<string, int> progressHandler = (msg, progress) => { } ;

        public static Start Work(Action callback)
        {
            return new Start 
            { 
                workHandlerWithProgress = 
                    (onprogress) => 
                    { 
                        callback(); 
                        return new object(); 
                    } 
            };
        }

        public static Start Work(Action<Action<string,int>> callback)
        {
            return new Start
            {
                workHandlerWithProgress =
                    (onprogress) =>
                    {
                        callback((msg, progress) => 
                        { 
                            onprogress(default(object), msg, progress); 
                        });

                        return new object();
                    }
            };
        }

        public Start OnComplete(Action callback)
        {
            this.successHandler = callback;
            return this;
        }

        public Start OnException(Action<Exception> callback)
        {
            this.exceptionHandler = callback;
            return this;
        }

        public Start OnProgress(Action<string, int> callback)
        {
            this.progressHandler = callback;
            return this;
        }

        public void Run()
        {
            ParallelWork.StartNow<object, object>(new object(),
                (o, progressCallback) => 
                { 
                    this.workHandlerWithProgress(progressCallback); 
                    return new object(); 
                },
                (o, msg, done) => { this.progressHandler(msg, done); },
                (o, result) => { this.successHandler(); },
                (o, x) => { this.exceptionHandler(x); });
        }

        public void RunAfter(TimeSpan duration)
        {
            ParallelWork.StartAfter<object, object>(new object(),
                (o, progressCallback) => 
                { 
                    this.workHandlerWithProgress(progressCallback); 
                    return new object(); 
                },
                (o, msg, done) => { this.progressHandler(msg, done); },
                (o, result) => { this.successHandler(); },
                (o, x) => { this.exceptionHandler(x); },
                duration);
        }
    }

    public class Start<T, R>
    {
        private Func<T, Action<T, string, int>, R> workHandler;
        private Action<T,R> successHandler = (arg, result) => { };
        private Action<T, Exception> exceptionHandler = (arg, x) => { };
        private Action<T, string, int> progressHandler = (arg, msg, progress) => { };

        public static Start<T,R> Work(Func<T, Action<T, string, int>, R> callback)
        {
            return new Start<T,R> { workHandler = callback };
        }

        public Start<T,R> OnComplete(Action<T,R> callback)
        {
            this.successHandler = callback;
            return this;
        }

        public Start<T,R> OnException(Action<T, Exception> callback)
        {
            this.exceptionHandler = callback;
            return this;
        }

        public Start<T,R> OnProgress(Action<T, string, int> callback)
        {
            this.progressHandler = callback;
            return this;
        }

        public void RunNow(T arg)
        {
            ParallelWork.StartNow<T, R>(arg,
                this.workHandler,
                this.progressHandler,
                this.successHandler,
                this.exceptionHandler);
        }

        public void RunAfter(T arg, TimeSpan duration)
        {
            ParallelWork.StartAfter<T, R>(arg,
                this.workHandler,
                this.progressHandler,
                this.successHandler,
                this.exceptionHandler,
                duration);
        }
    }

    public class Start<R>
    {
        private Func<Action<string, int>, R> workHandlerWithProgress;
        private Action<R> successHandler = (result) => { };
        private Action<Exception> exceptionHandler = (x) => { };
        private Action<string, int> progressHandler = (msg, progress) => { };

        public static Start<R> Work(Func<R> callback)
        {
            return new Start<R> 
            { 
                workHandlerWithProgress = (onprogress) => 
                { 
                    return callback(); 
                } 
            };
        }

        public static Start<R> Work(Func<Action<string,int>, R> callback)
        {
            return new Start<R> { workHandlerWithProgress = callback };
        }

        public Start<R> OnComplete(Action<R> callback)
        {
            this.successHandler = callback;
            return this;
        }

        public Start<R> OnException(Action<Exception> callback)
        {
            this.exceptionHandler = callback;
            return this;
        }

        public Start<R> OnProgress(Action<string, int> callback)
        {
            this.progressHandler = callback;
            return this;
        }

        public void Run()
        {
            ParallelWork.StartNow<object, R>(default(object),
                (arg, onprogress) => 
                {
                    return this.workHandlerWithProgress((msg, done) =>
                        {
                            onprogress(default(object), msg, done);
                        });
                },
                (arg, msg, done) => { this.progressHandler(msg, done); },
                (arg, result) => { this.successHandler(result); },
                (arg, x) => { this.exceptionHandler(x); });
        }

        public void RunAfter(TimeSpan duration)
        {
            ParallelWork.StartAfter<object, R>(default(object),
                (arg, onprogress) =>
                {
                    return this.workHandlerWithProgress((msg, done) =>
                    {
                        onprogress(default(object), msg, done);
                    });
                },
                (arg, msg, done) => { this.progressHandler(msg, done); },
                (arg, result) => { this.successHandler(result); },
                (arg, x) => { this.exceptionHandler(x); },
                duration);
        }
    }
}
