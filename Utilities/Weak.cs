using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    using System;
    using System.Runtime.InteropServices;

    public class Weak<T> : IDisposable        
    {
        private GCHandle handle;
        private bool trackResurrection;

        public Weak(T target)
            : this(target, false)
        {
        }

        public Weak(T target, bool trackResurrection)
        {
            this.trackResurrection = trackResurrection;
            this.Target = target;
        }

        ~Weak()
        {
            Dispose();
        }

        public void Dispose()
        {
            handle.Free();
            GC.SuppressFinalize(this);
        }

        public virtual bool IsAlive
        {
            get { return (handle.Target != null); }
        }

        public virtual bool TrackResurrection
        {
            get { return this.trackResurrection; }
        }

        public virtual T Target
        {
            get
            {
                object o = handle.Target;
                if ((o == null) || (!(o is T)))
                    return default(T);
                else
                    return (T)o;
            }
            set
            {
                if (handle != null)
                    if (handle.IsAllocated)
                        handle.Free();

                handle = GCHandle.Alloc(value,
                  this.trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);
            }
        }

        public static implicit operator Weak<T>(T obj)  
        {
            return new Weak<T>(obj);
        }

        public static implicit operator T(Weak<T> weakRef)  
        {
            return weakRef.Target;
        }
    }
}
