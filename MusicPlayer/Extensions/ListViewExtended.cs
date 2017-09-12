////using System;
////using System.Collections.Specialized;

////namespace MusicPlayer.Extensions
////{
////    /// <summary>
////    /// Extends the list view so a scroll event exists
////    /// </summary>
////    public class ListViewExtended : //// ListView
////    {

////     //    public delegate void ListViewItemDelegate(ListViewItem item); 
////     //public delegate void ListViewItemRangeDelegate(ListViewItem[] item); 
////     //public delegate void ListViewRemoveDelegate(ListViewItem item); 
////     //public delegate void ListViewRemoveAtDelegate(int index,
////     //     ListViewItem item); 

////     ////Next come the event declarations:

////     //public event ListViewItemDelegate ItemAdded; 
////     //public event ListViewItemRangeDelegate ItemRangeAdded; 
////     //public event ListViewRemoveDelegate ItemRemoved; 
////     //public event ListViewRemoveAtDelegate ItemRemovedAt; 

////     ////Now explicitly hide the derived Items propery by declaring it as new:

////     //public ListViewItemCollection Items; 

////     ////The Constructor and the initialisation of the new
////     ////ListViewItemCollection 

////     //public ListViewExtended():base() 
////     //{ 
////     //     Items = new ListViewItemCollection(this); 
////     //} 

////     ////Next we provide the methods that the extended
////     ////"ListViewItemCollection" inner 
////     ////class will call and inside their implementation we 
////     ////raise our events to notify the Observers. 
////     //private void AddedItem(ListViewItem lvi) 
////     //{ 
////     //     this.ItemAdded(lvi); 
////     //} 
////     //private void AddedItemRange(ListViewItem[] lvi) 
////     //{ 
////     //     this.ItemRangeAdded(lvi); 
////     //} 
////     //private void RemovedItem(ListViewItem lvi) 
////     //{  
////     //     this.ItemRemoved(lvi); 
////     //} 
////     //private void RemovedItem(int index, ListViewItem item) 
////     //{ 
////     //     this.ItemRemovedAt(index, item); 
////     //} 

////        public event ScrollEventHandler Scroll;

////        protected virtual void OnScroll(ScrollEventArgs e)
////        {
////            ScrollEventHandler handler = this.Scroll;
////            if (handler != null) handler(this, e);
////        }
////        protected override void WndProc(ref Message m)
////        {
////            base.WndProc(ref m);

////            // Scrollbar
////            if (m.Msg == 0x115)
////            { // Trap WM_VSCROLL
////                OnScroll(new ScrollEventArgs((ScrollEventType)(m.WParam.ToInt32() & 0xffff), 0));
////            }

////            //mouseWheel
////            if (m.Msg == 0x020A)
////            {
////                OnScroll(new ScrollEventArgs((ScrollEventType)(m.WParam.ToInt64() & 0xffff), 0));
////            }
////        }
////    }
////}
