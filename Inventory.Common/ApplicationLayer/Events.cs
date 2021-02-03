namespace Inventory.Common.ApplicationLayer {
    using Inventory.Common.EntityLayer.Model.Entities;
    using Prism.Events;

    public class ThemeChangeEvent:PubSubEvent<string> {}
    public class CreateNewUserEvent : PubSubEvent<User> { }
    public class UserUpdatedEvent : PubSubEvent<User> { }
    public class UserModuleUpdateEvent : PubSubEvent { }

    public class WarehouseSaveEvent : PubSubEvent<Warehouse> { }
    public class WarehouseUpdateEvent : PubSubEvent<Warehouse> { }
    public class WarehouseDiscardEvent : PubSubEvent{ }

    public class LocationSaveEvent : PubSubEvent<Location> { }
    public class LocationUpdateEvent : PubSubEvent<Location> { }
    public class LocationDiscardEvent : PubSubEvent { }

    public class ConsumerDiscardEvent : PubSubEvent { }
    public class ConsumerUpdateEvent : PubSubEvent<Consumer> { }
    public class ConsumerSaveEvent : PubSubEvent<Consumer> { }

    //Products

    public class IncomingLotCarrier {
        public Lot Lot { get; set; }
        public string RMA { get; set; }
    }

    public class LotCallbackCarrier {
        public Lot Lot { get; set; }
        public bool Success { get; set; }
    }

    public class RankCallbackCarrier {
        public ProductInstance Rank { get; set; }
        public bool Success { get; set; }
    }

    public class CancelOutgoingListEvent : PubSubEvent { }
    public class DoneOutgoingListEvent : PubSubEvent { }
    public class StartOutgoingListEvent : PubSubEvent { }
    public class AddToOutgoingEvent : PubSubEvent<ProductInstance> { }
    public class BatchImportRunning : PubSubEvent { }
    public class BatchImportFinished : PubSubEvent { }


    public class CancelIncomingListEvent : PubSubEvent { }
    public class DoneIncomingListEvent : PubSubEvent { }
    public class StartIncomingListEvent : PubSubEvent { }
    public class SetInIncomingFormEvent : PubSubEvent<Product> { }
    public class AddToIncomingEvent : PubSubEvent<IncomingLotCarrier> { }
    public class AddToIncomingCallback : PubSubEvent<bool> { }

    public class ProductEditingDoneEvent : PubSubEvent { }

    public class RenameHeaderEvent : PubSubEvent<string> { }

    public class LotRankReservationEditingStartedEvent : PubSubEvent { }
    public class LotRankReservationEditingDoneEvent:PubSubEvent { }

    public class ProductReservationEditDoneEvent : PubSubEvent { }

    public class ReloadCategoriesEvent : PubSubEvent { }
    public class SaveNewCategoryEvent : PubSubEvent<Category> { }
    public class SaveCategoryEvent : PubSubEvent<Category> { }
    public class DiscardCategoryEvent : PubSubEvent { }
    public class ModifyCategoryCallBackEvent : PubSubEvent { }

}
