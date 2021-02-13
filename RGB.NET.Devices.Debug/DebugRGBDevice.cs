﻿using System;
using System.Collections.Generic;
using RGB.NET.Core;
using RGB.NET.Layout;

namespace RGB.NET.Devices.Debug
{
    /// <inheritdoc cref="AbstractRGBDevice{TDeviceInfo}" />
    /// <summary>
    /// Represents a debug device.
    /// </summary>
    public class DebugRGBDevice : AbstractRGBDevice<DebugRGBDeviceInfo>, IUnknownDevice
    {
        #region Properties & Fields

        /// <inheritdoc />
        public override DebugRGBDeviceInfo DeviceInfo { get; }

        public IDeviceLayout Layout { get; }

        private Action<IEnumerable<Led>>? _updateLedsAction;

        #endregion

        #region Constructors
        /// <summary>
        /// Internal constructor of <see cref="DebugRGBDeviceInfo"/>.
        /// </summary>
        internal DebugRGBDevice(IDeviceLayout layout, Action<IEnumerable<Led>>? updateLedsAction = null)
        {
            this.Layout = layout;
            this._updateLedsAction = updateLedsAction;

            DeviceInfo = new DebugRGBDeviceInfo(layout.Type, layout.Vendor ?? "RGB.NET", layout.Model ?? "Debug", layout.CustomData);

            Layout.ApplyTo(this, true);
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        protected override void UpdateLeds(IEnumerable<Led> ledsToUpdate) => _updateLedsAction?.Invoke(ledsToUpdate);

        #endregion
    }
}
