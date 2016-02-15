using System.Collections.Generic;

namespace Mapper.Utilities
{
    /// <summary>
    /// Defines the contract for an object that has a parent object
    /// </summary>
    /// <typeparam name="T">Type of the parent object</typeparam>
    public interface IChildItem<T> where T : class
    {
        T Parent { get; set; }
    }


    /// <summary>
    /// Collection of child items. This collection automatically set the
    /// Parent property of the child items when they are added or removed
    /// </summary>
    /// <typeparam name="TParent">Type of the parent object</typeparam>
    /// <typeparam name="TChild">Type of the child items</typeparam>
    public class ChildItemCollection<TParent, TChild> : IList<TChild>
        where TParent : class
        where TChild : IChildItem<TParent>
    {
        private readonly TParent parent;
        private readonly IList<TChild> collection;

        public ChildItemCollection(TParent parent)
        {
            this.parent = parent;
            this.collection = new List<TChild>();
        }

        public ChildItemCollection(TParent parent, IList<TChild> collection)
        {
            this.parent = parent;
            this.collection = collection;
        }

        #region IList<T> Members

        public int IndexOf(TChild item)
        {
            return collection.IndexOf(item);
        }

        public void Insert(int index, TChild item)
        {
            if (item != null)
                item.Parent = parent;
            collection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            TChild oldItem = collection[index];
            collection.RemoveAt(index);
            if (oldItem != null)
                oldItem.Parent = null;
        }

        public TChild this[int index]
        {
            get
            {
                return collection[index];
            }
            set
            {
                TChild oldItem = collection[index];
                if (value != null)
                    value.Parent = parent;
                collection[index] = value;
                if (oldItem != null)
                    oldItem.Parent = null;
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(TChild item)
        {
            if (item != null)
                item.Parent = parent;
            collection.Add(item);
        }

        public void Clear()
        {
            foreach (TChild item in collection)
            {
                if (item != null)
                    item.Parent = null;
            }
            collection.Clear();
        }

        public bool Contains(TChild item)
        {
            return collection.Contains(item);
        }

        public void CopyTo(TChild[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return collection.IsReadOnly; }
        }

        public bool Remove(TChild item)
        {
            bool b = collection.Remove(item);
            if (item != null)
                item.Parent = null;
            return b;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<TChild> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (collection as System.Collections.IEnumerable).GetEnumerator();
        }

        #endregion
    }
}
