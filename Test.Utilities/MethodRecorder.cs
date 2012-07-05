using System;
using System.Dynamic;
using System.Reflection;

namespace Unit.Tests
{
	/// <summary>
	/// Class that records methods invoked on a proxy.
	/// </summary>
	/// <typeparam name="T">The type that is being proxied</typeparam>
	public class MethodRecorder<T> : DynamicObject
	{
		private readonly T _proxied;

		/// <summary>
		/// Creates a new MethodRecorder.
		/// </summary>
		public MethodRecorder()
		{
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return base.TryGetMember(binder, out result);
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			return base.TrySetMember(binder, value);
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			return base.TryInvokeMember(binder, args, out result);
		}

		public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
		{
			return base.TryInvoke(binder, args, out result);
		}

		/// <summary>
		/// Records a method invocation on the proxied member.
		/// </summary>
		/// <param name="invocation">The recorded method invocation</param>
		public MemberInfo Record(Action<T> invocation)
		{
			return Record(invocation, this);
		}

		/// <summary>
		/// Records a method invocation on the proxied member.
		/// </summary>
		/// <param name="invocation">The recorded method invocation</param>
		public MemberInfo Record<V>(Func<T, V> invocation)
		{
			return Record(invocation, this);
		}

		private MemberInfo Record(Delegate invocation, dynamic argument)
		{
			try
			{
				invocation.DynamicInvoke(argument);
			}
			catch (TargetException) { }
			catch (NotSupportedException) { }

			return LastInvocation;
		}

		/// <summary>
		/// The last recorded invoked member.
		/// </summary>
		public MemberInfo LastInvocation { get; private set; }
	}
}
