using System;
using CQRSlite.Events;

namespace CQRSCode.ReadModel.Events
{
    public class InventoryItemDeactivated : EventBase
    {
        public InventoryItemDeactivated(Guid id)
        {
            Id = id;
        }
	}
}