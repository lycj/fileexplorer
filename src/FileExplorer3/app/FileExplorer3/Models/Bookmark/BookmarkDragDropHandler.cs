using FileExplorer.Defines;
using FileExplorer.UIEventHub;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models.Bookmark
{
    public class BookmarkDragDropHandler : IDragDropHandler
    {
        public ISupportDrag GetDragHelper(IEnumerable<IEntryModel> entries)
        {
            return new BookmarkDragHelper(entries);
        }

        public ISupportDrop GetDropHelper(IEntryModel destEm)
        {
            return new BookmarkDropHelper((BookmarkModel)destEm);
        }
    }

    public class BookmarkDragHelper : DragHelper
    {
        private IEnumerable<IEntryModel> _dragSources;
        public BookmarkDragHelper(IEnumerable<IEntryModel> dragSources)
        {
            _dragSources = dragSources;
            HasDraggables = dragSources.Count() > 0;
        }

        public override IEnumerable<IDraggable> GetDraggables()
        {
            return _dragSources.Cast<IDraggable>();
        }

        public override DragDropEffectsEx QueryDrag(IEnumerable<IDraggable> draggables)
        {
            BookmarkProfile profile = (draggables.First() as BookmarkModel).Profile as BookmarkProfile;

            if (draggables.Any(d => d.Equals(profile.RootModel)))
                return DragDropEffectsEx.None; //Root not draggable.

            return DragDropEffectsEx.Move;
        }

        public override void OnDragCompleted(IEnumerable<IDraggable> draggables, DragDropEffectsEx effect)
        {
            if (effect == DragDropEffectsEx.Move)
                foreach (var b in draggables.Where(d => d is BookmarkModel).Cast<BookmarkModel>())
                {
                    BookmarkModel parentModel = b.Parent as BookmarkModel;
                    if (parentModel != null)
                        parentModel.Remove(b.Label);
                    
                    (parentModel.Profile as BookmarkProfile).RaiseEntryChanged(
                        new EntryChangedEvent(ChangeType.Deleted, b.FullPath));
                }

        }
    }

    public class BookmarkDropHelper : DropHelper
    {
        private BookmarkModel _dropTarget;
        public BookmarkDropHelper(BookmarkModel dropTarget)
        {
            IsDroppable = true;
            _dropTarget = dropTarget;
        }

        public override QueryDropEffects QueryDrop(IEnumerable<IDraggable> draggables, Defines.DragDropEffectsEx allowedEffects)
        {
            if (_dropTarget.SubModels == null ||
                draggables.Any(d => 
                    _dropTarget.Equals(d) ||
                    _dropTarget.SubModels.Contains(d)))
                return QueryDropEffects.None;

            if (draggables.Any(d => d is BookmarkModel))
                return new QueryDropEffects(DragDropEffectsEx.Move, DragDropEffectsEx.Move);
            if (allowedEffects.HasFlag(DragDropEffectsEx.Link))
                return new QueryDropEffects(DragDropEffectsEx.Link, DragDropEffectsEx.Link);
            return QueryDropEffects.None;
        }

        private void add(BookmarkModel source, BookmarkModel target)
        {
            var addedModel =
                (source.Type == BookmarkModel.BookmarkEntryType.Link) ?
                target.AddLink(source.Label, source.LinkPath) :
                target.AddFolder(source.Label);

            if (source.Type == BookmarkModel.BookmarkEntryType.Directory)
                foreach (var sub in source.SubModels)
                    add(sub, addedModel);
        }

        public override DragDropEffectsEx Drop(IEnumerable<IDraggable> draggables, DragDropEffectsEx allowedEffects)
        {
            foreach (var e in draggables.Where(d => d is IEntryModel).Cast<IEntryModel>())
            {
                BookmarkModel be = e as BookmarkModel;
                if (be != null)
                {
                    add(be, _dropTarget as BookmarkModel);

                }
                else
                    _dropTarget.AddLink(e.Label, e.FullPath);
            }

            return allowedEffects;
        }

    }
}
