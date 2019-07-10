using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Common.DataLayer.Services;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.BuisnessLayer;
using EntityFramework.Triggers;


namespace Inventory.Common.DataLayer {

    public class PartTriggers : IEntityTriggers<Part, InventoryContext> {

        private static readonly Lazy<AttachmentTriggers> lazy = new Lazy<AttachmentTriggers>(() => new AttachmentTriggers());
        private FileService _fileService;

        public PartTriggers()
        {
            this._fileService = new FileService();
            Triggers<Part, InventoryContext>.Inserting += this.OnInsert;
            Triggers<Part, InventoryContext>.Deleting += this.OnDelete;
            Triggers<Part, InventoryContext>.Updating += this.OnUpdate;
        }


        public void OnDelete(IDeletingEntry<Part, InventoryContext> entry)
        {

        }

        public void OnInsert(IInsertingEntry<Part, InventoryContext> entry)
        {

        }

        public void OnUpdate(IUpdatingEntry<Part, InventoryContext> entry)
        {

        }
    }

    public class AttachmentTriggers:IEntityTriggers<Attachment,InventoryContext> {

        private static readonly Lazy<AttachmentTriggers> lazy = new Lazy<AttachmentTriggers>(() => new AttachmentTriggers());
        private FileService _fileService;

        public AttachmentTriggers()
        {
            this._fileService = new FileService();
            Triggers<Attachment, InventoryContext>.Inserting += this.OnInsert;
            Triggers<Attachment, InventoryContext>.Deleting += this.OnDelete;
            Triggers<Attachment, InventoryContext>.Updating += this.OnUpdate;
        }


        public void OnDelete(IDeletingEntry<Attachment, InventoryContext> entry) {

        }

        public void OnInsert(IInsertingEntry<Attachment, InventoryContext> entry) {
            entry.Entity.FileReference=this._fileService.SaveFile(entry.Entity.Name,entry.Entity.SourceReference);
        }

        public void OnUpdate(IUpdatingEntry<Attachment, InventoryContext> entry) {

        }
    }
}
