﻿using System;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace MaterialIconsGenerator.Common
{
    public static class UIThreadHelper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static void SetCustomJoinableTaskFactory(Thread mainThread, SynchronizationContext synchronizationContext)
        {
            // If ThreadHelper.JoinableTaskFactory is not null, do not allow a custom JoinableTaskFactory to be set here
            if (GetThreadHelperJoinableTaskFactorySafe() != null)
            {
                throw new ArgumentException("CustomJoinableTaskFactoryNotAllowed");
            }

            if (mainThread == null)
            {
                throw new ArgumentNullException(nameof(mainThread));
            }

            if (synchronizationContext == null)
            {
                throw new ArgumentNullException(nameof(synchronizationContext));
            }

            // This method is not thread-safe and does not have it to be
            // This is really just a test-hook to be used by test standalone UI and only 1 thread will call into this
            // And, note that this method throws, when running inside VS, and ThreadHelper.JoinableTaskContext is not null
            var joinableTaskContext = new JoinableTaskContext(mainThread, synchronizationContext);
            _joinableTaskFactory = joinableTaskContext.Factory;
        }

        private static JoinableTaskFactory _joinableTaskFactory;

        public static JoinableTaskFactory JoinableTaskFactory
        {
            get { return GetThreadHelperJoinableTaskFactorySafe() ?? _joinableTaskFactory; }
        }

        private static JoinableTaskFactory GetThreadHelperJoinableTaskFactorySafe()
        {
            // Static getter ThreadHelper.JoinableTaskContext, throws NullReferenceException if VsTaskLibraryHelper.ServiceInstance is null
            // And, ThreadHelper.JoinableTaskContext is simply 'ThreadHelper.JoinableTaskContext?.Factory'. Hence, this helper
            return VsTaskLibraryHelper.ServiceInstance != null ? ThreadHelper.JoinableTaskFactory : null;
        }
    }
}
