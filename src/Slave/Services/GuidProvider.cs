using System;

namespace Slave.Services
{
    public class GuidProvider
    {

        private readonly Lazy<Guid> _id;

        public GuidProvider() => _id = new Lazy<Guid>(Guid.NewGuid());
        public Guid Id => _id.Value;
    }

}
