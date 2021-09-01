namespace MakeSimple.SharedKernel.Wrappers
{
    using MakeSimple.SharedKernel.Contract;
    using System.Collections.Generic;

    public class Response<TResponse> : ValueObject
    {
        public TResponse Item { get; private set; }

        public Response(TResponse item)
        {
            Item = item;
        }

        public Response()
        {
            Item = default;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Item;
        }
    }
}