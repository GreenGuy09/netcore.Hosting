﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace netcore.Hosting.Internal
{
    /// <summary>
    /// Allows consumers to perform cleanup during a graceful shutdown.
    /// </summary>
    public class ApplicationLifetime : IApplicationLifetime
    {
        private readonly CancellationTokenSource _startedSource = new CancellationTokenSource();
        private readonly CancellationTokenSource _stoppingSource = new CancellationTokenSource();
        private readonly CancellationTokenSource _stoppedSource = new CancellationTokenSource();
        private readonly ILogger<ApplicationLifetime> _logger;

        public ApplicationLifetime(ILogger<ApplicationLifetime> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Triggered when the application host has fully started and is about to wait
        /// for a graceful shutdown.
        /// </summary>
        public CancellationToken ApplicationStarted => _startedSource.Token;

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// Request may still be in flight. Shutdown will block until this event completes.
        /// </summary>
        public CancellationToken ApplicationStopping => _stoppingSource.Token;

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// All requests should be complete at this point. Shutdown will block
        /// until this event completes.
        /// </summary>
        public CancellationToken ApplicationStopped => _stoppedSource.Token;

        /// <summary>
        /// Signals the ApplicationStopping event and blocks until it completes.
        /// </summary>
        public void StopApplication()
        {
            // Lock on CTS to synchronize multiple calls to StopApplication. This guarantees that the first call 
            // to StopApplication and its callbacks run to completion before subsequent calls to StopApplication, 
            // which will no-op since the first call already requested cancellation, get a chance to execute.
            lock (_stoppingSource)
            {
                try
                {
                    ExecuteHandlers(_stoppingSource);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred starting the application - {0}", ex.Message);
                }
            }
        }

        /// <summary>
        /// Signals the ApplicationStarted event and blocks until it completes.
        /// </summary>
        public void NotifyStarted()
        {
            try
            {
                ExecuteHandlers(_startedSource);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred starting the application - {0}", ex.Message);
            }
        }

        /// <summary>
        /// Signals the ApplicationStopped event and blocks until it completes.
        /// </summary>
        public void NotifyStopped()
        {
            try
            {
                ExecuteHandlers(_stoppedSource);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred starting the application - {0}", ex.Message);
            }
        }

        private void ExecuteHandlers(CancellationTokenSource cancel)
        {
            // Noop if this is already cancelled
            if (cancel.IsCancellationRequested)
            {
                return;
            }

            List<Exception> exceptions = null;

            try
            {
                // Run the cancellation token callbacks
                cancel.Cancel(throwOnFirstException: false);
            }
            catch (Exception ex)
            {
                if (exceptions == null)
                {
                    exceptions = new List<Exception>();
                }

                exceptions.Add(ex);
            }

            // Throw an aggregate exception if there were any exceptions
            if (exceptions != null)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}
