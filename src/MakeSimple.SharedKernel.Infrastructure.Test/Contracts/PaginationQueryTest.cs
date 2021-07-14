
namespace MakeSimple.SharedKernel.Infrastructure.Test.Contracts
{
    using MakeSimple.SharedKernel.Contract;
    using Xunit;
    public class PaginationQueryTest
    {
        public const int MaxPageSetting = 1000;
        public const int DefaultPageSizeSetting = 10000; 
        public class Implement : PaginationQuery
        {
            public override int MaxPageSize { get; protected set; } = MaxPageSetting;
            public override int DefaultPageSize { get; protected set; } = DefaultPageSizeSetting;
        }

        [Fact]
        public void PaginationQuery_Overrride_SuccessAsync()
        {
            // Arrange
            Implement u = new Implement();

            // Act

            // Assert
            Assert.True(u.MaxPageSize == MaxPageSetting);
            Assert.True(u.DefaultPageSize == DefaultPageSizeSetting);
        }
    }
}
