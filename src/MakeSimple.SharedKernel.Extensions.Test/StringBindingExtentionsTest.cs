using System;
using Xunit;

namespace MakeSimple.SharedKernel.Extensions.Test
{
    public class StringBindingExtentionsTest
    {
        [Fact]
        public void Test_AnonymousType_Binding_With_Format()
        {
            // Arrange
            var Template = "Tran Van {Name}, Sinh nhat: {Birthday:dd-MM-yyyy}, Tien: {Money:G}";
            var Data = new
            {
                Name = "Thao",
                Birthday = DateTime.Now,
                Money = 20
            };
            // Act
            var actual = Template.BindObjectProperties(Data);
            var expected = $"Tran Van Thao, Sinh nhat: {Data.Birthday:dd-MM-yyyy}, Tien: {Data.Money:G}";
            // Assert
            Assert.True(actual == expected);
        }

        [Fact]
        public void Test_AnonymousType_Binding_Without_Format()
        {
            // Arrange
            var Template = "Tran Van {Name}";
            var Data = new
            {
                Name = "Thao"
            };
            // Act
            var actual = Template.BindObjectProperties(Data);
            var expected = $"Tran Van Thao";
            // Assert
            Assert.True(actual == expected);
        }

        [Fact]
        public void Test_JsonElementType_Binding_With_Format()
        {
            // Arrange
            var Template = "Tran Van {Name}, Sinh nhat: {Birthday:dd-MM-yyyy}, Tien: {Money:G}";
            var Data = new
            {
                Name = "Thao",
                Birthday = DateTime.Now,
                Money = 20
            };

            var elementObj = (System.Text.Json.JsonElement)System.Text.Json.JsonSerializer.Deserialize<object>(System.Text.Json.JsonSerializer.Serialize(Data));
            // Act
            var actual = Template.BindObjectProperties(elementObj);
            var expected = $"Tran Van Thao, Sinh nhat: {Data.Birthday:dd-MM-yyyy}, Tien: {Data.Money:G}";
            // Assert
            Assert.True(actual == expected);
        }

        [Fact]
        public void Test_JsonElementType_Binding_Without_Format()
        {
            // Arrange
            var Template = "Tran Van {Name}";
            var Data = new
            {
                Name = "Thao"
            };

            var elementObj =(System.Text.Json.JsonElement) System.Text.Json.JsonSerializer.Deserialize<object>(System.Text.Json.JsonSerializer.Serialize(Data));
            // Act
            var actual = Template.BindObjectProperties(elementObj);
            var expected = $"Tran Van Thao";
            // Assert
            Assert.True(actual == expected);
        }
    }
}
