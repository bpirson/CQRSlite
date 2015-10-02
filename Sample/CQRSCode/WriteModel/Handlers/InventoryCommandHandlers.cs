using System.Threading.Tasks;
using CQRSCode.WriteModel.Commands;
using CQRSCode.WriteModel.Domain;
using CQRSlite.Commands;
using CQRSlite.Domain;

namespace CQRSCode.WriteModel.Handlers
{
    public class InventoryCommandHandlers : ICommandHandler<CreateInventoryItem>,
											ICommandHandler<DeactivateInventoryItem>,
											ICommandHandler<RemoveItemsFromInventory>,
											ICommandHandler<CheckInItemsToInventory>,
											ICommandHandler<RenameInventoryItem>
    {
        private readonly ISession _session;

        public InventoryCommandHandlers(ISession session)
        {
            _session = session;
        }

        public async Task HandleAsync(CreateInventoryItem message)
        {
            var item = new InventoryItem(message.Id, message.Name);
            _session.Add(item);
            await _session.CommitAsync();
        }

        public async Task HandleAsync(DeactivateInventoryItem message)
        {
            var item = _session.Get<InventoryItem>(message.Id, message.ExpectedVersion);
            item.Deactivate();
            await _session.CommitAsync();
        }

        public async Task HandleAsync(RemoveItemsFromInventory message)
        {
            var item = _session.Get<InventoryItem>(message.Id, message.ExpectedVersion);
            item.Remove(message.Count);
            await _session.CommitAsync();
        }

        public async Task HandleAsync(CheckInItemsToInventory message)
        {
            var item = _session.Get<InventoryItem>(message.Id, message.ExpectedVersion);
            item.CheckIn(message.Count);
           await _session.CommitAsync();
        }

        public async Task HandleAsync(RenameInventoryItem message)
        {
            var item = _session.Get<InventoryItem>(message.Id, message.ExpectedVersion);
            item.ChangeName(message.NewName);
            await _session.CommitAsync();
        }
    }
}
