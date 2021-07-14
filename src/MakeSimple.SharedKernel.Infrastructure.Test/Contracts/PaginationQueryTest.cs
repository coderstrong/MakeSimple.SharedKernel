
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
            protected override int MaxPageSize { get; } = MaxPageSetting;
            protected override int DefaultPageSize { get; set; } = DefaultPageSizeSetting;
        }

        [Fact]
        public void PaginationQuery_Overrride_SuccessAsync()
        {
            // Arrange
            Implement u = new Implement();
            u.PageNumber = 1;
            u.PageSize = DefaultPageSizeSetting;
            // Act

            // Assert
            Assert.True(u.PageSize == MaxPageSetting);
        }
    }
}
