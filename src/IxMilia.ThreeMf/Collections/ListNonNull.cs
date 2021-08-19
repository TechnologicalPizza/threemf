using System.Collections.Generic;

namespace IxMilia.ThreeMf.Collections
{
    public class ListNonNull<T> : ListWithPredicate<T>
    {
        public ListNonNull()
        {
        }

        public ListNonNull(IEnumerable<T> items) : base(items)
        {
        }

        protected override bool IsValidItem(T item)
        {
            return item != null;
        }
    }
}
