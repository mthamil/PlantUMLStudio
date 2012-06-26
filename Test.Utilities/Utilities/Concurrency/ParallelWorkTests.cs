using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using SubSpec;
using Utilities.Concurrency;
using Xunit;

namespace Unit.Tests.Utilities.Concurrency
{
    public class ParallelWorkTests
    {
        [Specification][STAThread]
        public void WaitForAllWork_should_return_immediately_if_no_work_queued()
        {
            var stopWatch = new Stopwatch();
            "Given no work going on".Context(() =>
            {
                Assert.False(ParallelWork.IsWorkOrTimerQueued());
            });

            var result = default(bool);
            "When WaitForAllWork is called".Do(() =>
            {
                stopWatch.Start();
                result = ParallelWork.WaitForAllWork(TimeSpan.FromSeconds(1));
            });

            "It should return immediately without going into any wait period".Assert(() =>
            {
                Assert.True(stopWatch.Elapsed < TimeSpan.FromSeconds(1));
            });

            "It should return true".Assert(() =>
            {
                Assert.True(result);
            });
        }

        [Specification][STAThread]
        public void StartNow_should_queue_a_new_thread_to_do_the_work()
        {
            TimeSpan howLongWorkTakes = TimeSpan.FromSeconds(2);
            TimeSpan timeout = howLongWorkTakes.Add(TimeSpan.FromMilliseconds(500));
            
            var doWorkCalled = false;
            var successCallbackFired = false;
            var onExceptionFired = false;

            var doWorkThreadId = default(int);
            var onCompleteThreadId = default(int);
            var onExceptionThreadId = default(int);
            var letsThrowException = false;

            Stopwatch stopWatch = new Stopwatch();
            DispatcherFrame frame = default(DispatcherFrame);
                    
            Action callbackFiredOnDispatcher = () => {
                frame.Continue = false; // Dispatcher should stop now
            };

            "Given no parallel work running".Context(() =>
            {
                Assert.False(ParallelWork.IsWorkOrTimerQueued());                
                frame = new DispatcherFrame();

                doWorkCalled = false;
                successCallbackFired = false;
                onExceptionFired = false;

                doWorkThreadId = default(int);
                onCompleteThreadId = default(int);
                onExceptionThreadId = default(int);
                
                stopWatch.Reset();                
            });

            "When a new work is started by Start.Work().Run()".Do(() =>
            {
                var shouldThrowException = letsThrowException;

                Start.Work(() =>
                    {
                        doWorkThreadId = Thread.CurrentThread.ManagedThreadId;
                        doWorkCalled = true;

                        // Simulate some delay in background work
                        Thread.Sleep(howLongWorkTakes);

                        if (shouldThrowException)
                        {
                            throw new ApplicationException("Exception");
                        }
                    })
                    .OnComplete(() =>
                    {
                        onCompleteThreadId = Thread.CurrentThread.ManagedThreadId;
                        successCallbackFired = true;

                        callbackFiredOnDispatcher();
                    })
                    .OnException((x) =>
                    {
                        onExceptionThreadId = Thread.CurrentThread.ManagedThreadId;
                        onExceptionFired = true;

                        callbackFiredOnDispatcher();
                    })
                    .Run();

                stopWatch.Start();
            });

            "It should return control immediately without blocking the current thread".Assert(() =>
            {
                Assert.True(stopWatch.Elapsed < howLongWorkTakes, 
                    string.Format("{0}<{1}", stopWatch.Elapsed, howLongWorkTakes));
                Assert.True(WaitForWorkDoneAndFireCallback(timeout, frame));
            });

            "It should return true if IsWorkQueued is called".Assert(() =>
            {
                Assert.True(ParallelWork.IsWorkOrTimerQueued());
                Assert.True(WaitForWorkDoneAndFireCallback(timeout, frame));
            });

            "It should wait for the work to complete if WaitForAllWork is called".Assert(() =>
            {
                Assert.True(WaitForWorkDoneAndFireCallback(timeout, frame));

                // The work should finish within the duration it takes with max 1 sec buffer
                // for additional stuff xunit does.
                Assert.True(stopWatch.Elapsed < howLongWorkTakes.Add(TimeSpan.FromSeconds(1)));
            });

            "It should execute the work in a separate thread".Assert(() => 
            {
                Assert.True(WaitForWorkDoneAndFireCallback(timeout, frame));

                Assert.True(doWorkCalled);
                Assert.NotEqual(Thread.CurrentThread.ManagedThreadId, doWorkThreadId);
            });

            "It should fire onComplete on the same thread as the UI thread".Assert(() =>
            {
                Assert.True(WaitForWorkDoneAndFireCallback(timeout, frame));

                Assert.True(successCallbackFired);
                Assert.Equal(Thread.CurrentThread.ManagedThreadId, onCompleteThreadId);
            });
            
            "It should not fire onException if there's no exception".Assert(() =>
            {
                Assert.True(WaitForWorkDoneAndFireCallback(timeout, frame));

                Assert.False(onExceptionFired);

                letsThrowException = true; // This is for next assert                
            });

            "It should fire exception on UI thread".Assert(() =>
            {
                Assert.True(WaitForWorkDoneAndFireCallback(timeout, frame));

                Assert.False(successCallbackFired);
                Assert.True(onExceptionFired);
                Assert.Equal(Thread.CurrentThread.ManagedThreadId, onExceptionThreadId);
            });
        }

        [Specification][STAThread]
        public void DoWork_should_return_objects_from_parallel_thread_to_callbacks()
        {
            TimeSpan howLongWorkTakes = TimeSpan.FromSeconds(1);
            TimeSpan timeout = howLongWorkTakes.Add(TimeSpan.FromMilliseconds(500));

            DispatcherFrame frame = default(DispatcherFrame);

            "Given no parallel work running".Context(() =>
            {
                Assert.False(ParallelWork.IsWorkOrTimerQueued());
                frame = new DispatcherFrame();
            });

            var test = default(Dictionary<string,string>);
            
            var output = default(Dictionary<string, string>);
            "When StartNow<> is called".Do(() =>
            {
                Start<Dictionary<string, string>>.Work(() =>
                    {
                        test = new Dictionary<string, string>();
                        test.Add("test", "test");

                        return test;
                    })
                    .OnComplete((result) =>
                    {
                        output = result;
                        frame.Continue = false;
                    })
                    .Run();
            });

            @"It should return the object produced in separate thread 
            and the object should be modifiable"
                .Assert(() =>
                {
                    Assert.True(WaitForWorkDoneAndFireCallback(timeout, frame));

                    Assert.True(output.ContainsKey("test"));
                    Assert.DoesNotThrow(() => output.Add("another key", "another value"));
                });
        }

        [Specification]
        [STAThread]
        public void DoWorkAfter_should_execute_the_work_after_given_duration()
        {
            DateTime workStartedAt = default(DateTime);
            DateTime workCompletedAt = default(DateTime);
            DateTime countDownStartedAt = default(DateTime);
            TimeSpan waitDuration = TimeSpan.FromSeconds(2);
            TimeSpan howLongWorkTakes = TimeSpan.FromSeconds(1);
            TimeSpan timeout = waitDuration.Add(howLongWorkTakes.Add(TimeSpan.FromMilliseconds(500)));

            DispatcherFrame frame = default(DispatcherFrame);

            "Given no parallel work running".Context(() =>
            {
                Assert.False(ParallelWork.IsAnyWorkRunning());
                frame = new DispatcherFrame();

                workStartedAt = default(DateTime);
                workCompletedAt = default(DateTime);
            });

            "When Start.Work().RunAfter() is called".Do(() =>
            {
                Start.Work(() =>
                    {
                        workStartedAt = DateTime.Now;
                        Thread.Sleep(howLongWorkTakes);
                    })
                    .OnComplete(() =>
                    {
                        workCompletedAt = DateTime.Now;
                        frame.Continue = false;
                    })
                    .RunAfter(waitDuration);

                countDownStartedAt = DateTime.Now;
            });

            "It should not start the work until the duration has elapsed".Assert(() =>
            {
                Assert.Equal(default(DateTime), workStartedAt);
                Assert.Equal(default(DateTime), workCompletedAt);

                Assert.True(WaitForWorkDoneAndFireCallback(timeout, frame));
            });

            "It should start the work after duration has elapsed".Assert(() =>
            {
                Assert.True(WaitForWorkDoneAndFireCallback(timeout, frame));

                // Work should start within 500ms of start time
                DateTime expectedStartTime = countDownStartedAt.Add(waitDuration);
                Assert.InRange(workStartedAt,
                    expectedStartTime,
                    expectedStartTime.AddMilliseconds(500));

                // Work should end within 500ms of expected end time
                DateTime expectedEndTime = countDownStartedAt.Add(waitDuration).Add(howLongWorkTakes);
                Assert.InRange(workCompletedAt,
                    expectedEndTime,
                    expectedEndTime.AddMilliseconds(500));
            });
        }

        [Specification]
        [STAThread]
        public void StopAllWork_should_stop_all_parallel_work()
        {
            DateTime work1StartedAt = default(DateTime);
            DateTime work2StartedAt = default(DateTime);

            DateTime work1EndedAt = default(DateTime);
            DateTime work2EndedAt = default(DateTime);

            TimeSpan waitDuration = TimeSpan.FromSeconds(2);
            TimeSpan howLongWorkTakes = TimeSpan.FromSeconds(1);
            TimeSpan timeout = waitDuration.Add(howLongWorkTakes.Add(TimeSpan.FromMilliseconds(500)));

            DispatcherFrame frame = default(DispatcherFrame);
            
            "Given some parallel work running".Context(() =>
            {
                frame = new DispatcherFrame();
                Start.Work(() =>
                    {
                        work1StartedAt = DateTime.Now;
                        Thread.Sleep(howLongWorkTakes);
                        work1EndedAt = DateTime.Now;
                    })
                    .OnComplete(() =>
                    {
                        frame.Continue = false;
                    })
                    .Run();

                Start.Work(() =>
                    {
                        work2StartedAt = DateTime.Now;
                        Thread.Sleep(howLongWorkTakes);
                        work2EndedAt = DateTime.Now;
                    })
                    .OnComplete(() =>
                    {
                        frame.Continue = false;
                    })
                    .Run();
            });

            "When StopAllWork is called".Do(() =>
            {
                // Let the work be half way through
                Thread.Sleep((int)howLongWorkTakes.TotalMilliseconds / 2);
                ParallelWork.StopAll();
            });

            "It should stop all work from completing".Assert(() =>
            {   
                // Confirm the work started
                Assert.NotEqual(default(DateTime), work1StartedAt);
                Assert.NotEqual(default(DateTime), work2StartedAt);
                
                // Confirm the work did not end
                Assert.Equal(default(DateTime), work1EndedAt);
                Assert.Equal(default(DateTime), work2EndedAt);

                Thread.Sleep(timeout);
            });
        }


        [Specification]
        [STAThread]
        public void Should_fire_onprogress_on_UI_thread()
        {
            var onprogressMessages = new List<string>();
            var callerThreadId = default(int);
            var onprogressCalledOnThreadId = default(int);
            var frame = default(DispatcherFrame);

            "Given no parallel work running".Context(() =>
            {
                Assert.False(ParallelWork.IsWorkOrTimerQueued());
                callerThreadId = Thread.CurrentThread.ManagedThreadId;
                onprogressCalledOnThreadId = default(int);
                frame = new DispatcherFrame();
            });

            Func<int, string> getMessage = (progress) => "Message " + progress.ToString();

            "When a parallel work is queued and onprogress callback is fired".Do(() =>
            {
                Start.Work((onprogress) =>
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Thread.Sleep(100);
                            onprogress(getMessage(i), i);
                        }
                    })
                    .OnProgress((msg, progress) =>
                    {
                        onprogressCalledOnThreadId = Thread.CurrentThread.ManagedThreadId;
                        onprogressMessages.Add(msg);
                    })
                    .OnComplete(() =>
                    {
                        frame.Continue = false;
                    })
                    .Run();
            });

            "It should fire onprogress callback on the UI thread".Assert(() =>
            {
                WaitForWorkDoneAndFireCallback(TimeSpan.FromSeconds(5), frame);
                Assert.Equal(10, onprogressMessages.Count);
                Assert.Equal(callerThreadId, onprogressCalledOnThreadId);
            });
        }

        //[Specification]
        //[STAThread]
        //public void IsAnyWorkRunning_should_return_true_only_if_some_thread_running()
        //{
        //    DateTime workStartedAt = default(DateTime);
        //    TimeSpan waitDuration = TimeSpan.FromSeconds(2);
        //    TimeSpan howLongWorkTakes = TimeSpan.FromSeconds(1);
        //    TimeSpan timeout = waitDuration.Add(howLongWorkTakes.Add(TimeSpan.FromMilliseconds(500)));

        //    DispatcherFrame frame = default(DispatcherFrame);

        //    "Given a timed work queued that hasn't started yet".Context(() =>
        //    {
        //        Assert.False(ParallelWork.IsWorkOrTimerQueued());

        //        frame = new DispatcherFrame();
        //        workStartedAt = default(DateTime);

        //        ParallelWork.DoWorkAfter(() =>
        //            {
        //                workStartedAt = DateTime.Now;
        //                Thread.Sleep(howLongWorkTakes);
        //            },
        //            () =>
        //            {
        //                frame.Continue = false;
        //            }, waitDuration);                
        //    });
            
        //    var result = default(bool);
        //    "When IsAnyWorkRunning is called".Do(() =>
        //    {
        //        result = ParallelWork.IsAnyWorkRunning();
        //    });

        //    "It should return false if the work hasn't started yet".Assert(() =>
        //    {
        //        Assert.False(result);

        //        Assert.True(WaitForWorkDoneAndFireCallback(timeout, frame));
        //    });

        //    "It should return true when a work is still going on".Assert(() =>
        //    {
        //        Dispatcher.PushFrame(frame);

        //        Thread.Sleep(waitDuration);
        //        Thread.Sleep(howLongWorkTakes.Milliseconds / 2);

        //        Assert.NotEqual(default(DateTime), workStartedAt);
        //        Assert.True(result);

        //        Assert.True(WaitForWorkDoneAndFireCallback(timeout, frame));
        //    });
        //}

        private bool WaitForWorkDoneAndFireCallback(TimeSpan timeout, DispatcherFrame frame)
        {
            if (ParallelWork.WaitForAllWork(timeout))
            {
                // Let the Disptacher.BeginInvoke calls proceed
                Dispatcher.PushFrame(frame);
                return true;
            }
            else
            {
                // waiting timed out. Work did not finish on time.
                return false;
            }
        }

    }
}
