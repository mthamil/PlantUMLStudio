//using System;
//using AopAlliance.Intercept;

//namespace BioRad.D11.StudyManager.Test.Unit.Utilities
//{
//    /// <summary>
//    /// Method interceptor that records methods invoked on a proxy.
//    /// </summary>
//    public class MethodRecordingInterceptor : IInterceptor
//    {
//        /// <summary>
//        /// Creates a new interceptor that records method invocations.
//        /// </summary>
//        public MethodRecordingInterceptor()
//        {
//            ProceedAfterRecording = true;
//        }

//        #region IMethodInterceptor Members

//        /// <see cref="IMethodInterceptor.Invoke"/>
//        public object Invoke(IMethodInvocation invocation)
//        {
//            LastInvocation = invocation;

//            if (ProceedAfterRecording)
//                return invocation.Proceed();

//            Type returnType = invocation.Method.ReturnType;
//            if (returnType.IsValueType && returnType != typeof(void))	// can't create an instance of Void
//                return Activator.CreateInstance(returnType);

//            return null;
//        }

//        #endregion

//        #region IMethodRecorder Members

//        /// <summary>
//        /// The most recent invocation made on the interceptor.
//        /// </summary>
//        public IMethodInvocation LastInvocation { get; private set; }

//        #endregion

//        /// <summary>
//        /// Allows termination of the interceptor chain immediately after a method
//        /// invocation is recorded instead of proceeding to the next advisor.
//        /// </summary>
//        internal InterceptorChainSuppressionToken SuppressInterceptorChain()
//        {
//            return new InterceptorChainSuppressionToken(this);
//        }

//        /// <summary>
//        /// Whether the recorder should proceed to the next interceptor after recording
//        /// an invocation.
//        /// </summary>
//        internal bool ProceedAfterRecording { get; set; }

//        /// <summary>
//        /// Aids in suppressing proceeding with the advisor chain in a method recorder.
//        /// </summary>
//        internal class InterceptorChainSuppressionToken : IDisposable
//        {
//            private readonly MethodRecordingInterceptor _recorder;

//            public InterceptorChainSuppressionToken(MethodRecordingInterceptor recorder)
//            {
//                _recorder = recorder;
//                recorder.ProceedAfterRecording = false;
//            }

//            #region Implementation of IDisposable

//            public void Dispose()
//            {
//                _recorder.ProceedAfterRecording = true;
//            }

//            #endregion
//        }
//    }
//}
