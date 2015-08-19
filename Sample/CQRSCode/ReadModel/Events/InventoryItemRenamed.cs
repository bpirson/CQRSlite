using System;
using CQRSlite.Events;

namespace CQRSCode.ReadModel.Events
{
    public class InventoryItemRenamed : EventBase
    {
        public readonly string NewName;
 
        public InventoryItemRenamed(Guid id, string newName)
        {
            Id = id;
            NewName = newName;
        }
    }
}