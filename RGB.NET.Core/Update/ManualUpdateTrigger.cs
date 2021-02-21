﻿// ReSharper disable MemberCanBePrivate.Global

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RGB.NET.Core
{
    /// <inheritdoc />
    /// <summary>
    /// Represents an <see cref="T:RGB.NET.Core.IUpdateTrigger" />
    /// </summary>
    public sealed class ManualUpdateTrigger : AbstractUpdateTrigger
    {
        #region Properties & Fields

        private AutoResetEvent _mutex = new(false);
        private Task? UpdateTask { get; set; }
        private CancellationTokenSource? UpdateTokenSource { get; set; }
        private CancellationToken UpdateToken { get; set; }

        /// <summary>
        /// Gets the time it took the last update-loop cycle to run.
        /// </summary>
        public double LastUpdateTime { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualUpdateTrigger"/> class.
        /// </summary>
        /// <param name="autostart">A value indicating if the trigger should automatically <see cref="Start"/> right after construction.</param>
        public ManualUpdateTrigger()
        {
            Start();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the trigger if needed, causing it to performing updates.
        /// </summary>
        private void Start()
        {
            if (UpdateTask == null)
            {
                UpdateTokenSource?.Dispose();
                UpdateTokenSource = new CancellationTokenSource();
                UpdateTask = Task.Factory.StartNew(UpdateLoop, (UpdateToken = UpdateTokenSource.Token), TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }

        /// <summary>
        /// Stops the trigger if running, causing it to stop performing updates.
        /// </summary>
        private void Stop()
        {
            if (UpdateTask != null)
            {
                UpdateTokenSource?.Cancel();
                // ReSharper disable once MethodSupportsCancellation
                UpdateTask.Wait();
                UpdateTask.Dispose();
                UpdateTask = null;
            }
        }

        public void TriggerUpdate() => _mutex.Set();

        private void UpdateLoop()
        {
            OnStartup();

            while (!UpdateToken.IsCancellationRequested)
            {
                if (_mutex.WaitOne(100))
                {
                    long preUpdateTicks = Stopwatch.GetTimestamp();
                    OnUpdate();
                    LastUpdateTime = ((Stopwatch.GetTimestamp() - preUpdateTicks) / 10000.0);
                }
            }
        }

        /// <inheritdoc />
        public override void Dispose() => Stop();

        #endregion
    }
}