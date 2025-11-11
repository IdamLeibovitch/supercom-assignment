using Backend.Data;
using Backend.Data.Models;
using Backend.Models;
using FluentAssertions;
using Xunit;

namespace Backend.Tests.Mapping
{
    /// <summary>
    /// Tests to ensure TaskPriority enums in both Models and Data.Models namespaces are synchronized
    /// to prevent runtime exceptions when using MappingProfile.MapEnum
    /// </summary>
    public class EnumMappingTests
    {
        [Theory]
        [InlineData(Backend.Models.TaskPriority.Low, Backend.Data.Models.TaskPriority.Low)]
        [InlineData(Backend.Models.TaskPriority.Medium, Backend.Data.Models.TaskPriority.Medium)]
        [InlineData(Backend.Models.TaskPriority.High, Backend.Data.Models.TaskPriority.High)]
        [InlineData(Backend.Models.TaskPriority.Critical, Backend.Data.Models.TaskPriority.Critical)]
        public void MapEnum_ModelsToDataModels_MapsCorrectly(Backend.Models.TaskPriority source, Backend.Data.Models.TaskPriority expected)
        {
            // Act
            var result = MappingProfile.MapEnum<Backend.Models.TaskPriority, Backend.Data.Models.TaskPriority>(source);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(Backend.Data.Models.TaskPriority.Low, Backend.Models.TaskPriority.Low)]
        [InlineData(Backend.Data.Models.TaskPriority.Medium, Backend.Models.TaskPriority.Medium)]
        [InlineData(Backend.Data.Models.TaskPriority.High, Backend.Models.TaskPriority.High)]
        [InlineData(Backend.Data.Models.TaskPriority.Critical, Backend.Models.TaskPriority.Critical)]
        public void MapEnum_DataModelsToModels_MapsCorrectly(Backend.Data.Models.TaskPriority source, Backend.Models.TaskPriority expected)
        {
            // Act
            var result = MappingProfile.MapEnum<Backend.Data.Models.TaskPriority, Backend.Models.TaskPriority>(source);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void TaskPriority_BothEnumsHaveSameValues()
        {
            // Arrange
            var modelsValues = Enum.GetValues<Backend.Models.TaskPriority>();
            var dataModelsValues = Enum.GetValues<Backend.Data.Models.TaskPriority>();

            // Assert - Same number of values
            modelsValues.Length.Should().Be(dataModelsValues.Length, 
                "Both TaskPriority enums should have the same number of values");

            // Assert - Each value in Models has a corresponding value in Data.Models
            foreach (var modelValue in modelsValues)
            {
                var modelName = modelValue.ToString();
                var modelIntValue = (int)modelValue;

                Enum.IsDefined(typeof(Backend.Data.Models.TaskPriority), modelIntValue)
                    .Should().BeTrue($"Data.Models.TaskPriority should have a value with index {modelIntValue}");

                var dataModelValue = (Backend.Data.Models.TaskPriority)modelIntValue;
                dataModelValue.ToString().Should().Be(modelName, 
                    $"Enum values at index {modelIntValue} should have the same name");
            }
        }

        [Fact]
        public void TaskPriority_AllModelsValuesCanBeMappedToDataModels()
        {
            // Arrange
            var modelsValues = Enum.GetValues<Backend.Models.TaskPriority>();

            // Act & Assert
            foreach (var value in modelsValues)
            {
                Action act = () => MappingProfile.MapEnum<Backend.Models.TaskPriority, Backend.Data.Models.TaskPriority>(value);
                
                act.Should().NotThrow($"Mapping {value} should not throw an exception");
            }
        }

        [Fact]
        public void TaskPriority_AllDataModelsValuesCanBeMappedToModels()
        {
            // Arrange
            var dataModelsValues = Enum.GetValues<Backend.Data.Models.TaskPriority>();

            // Act & Assert
            foreach (var value in dataModelsValues)
            {
                Action act = () => MappingProfile.MapEnum<Backend.Data.Models.TaskPriority, Backend.Models.TaskPriority>(value);
                
                act.Should().NotThrow($"Mapping {value} should not throw an exception");
            }
        }

        [Fact]
        public void MapEnum_WithInvalidValue_ThrowsArgumentOutOfRangeException()
        {
            // Arrange - Create an invalid enum value
            var invalidValue = (Backend.Models.TaskPriority)999;

            // Act
            Action act = () => MappingProfile.MapEnum<Backend.Models.TaskPriority, Backend.Data.Models.TaskPriority>(invalidValue);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("*Unknown enum value*");
        }

        [Fact]
        public void TaskPriority_EnumNamesMatch()
        {
            // Arrange
            var modelsNames = Enum.GetNames<Backend.Models.TaskPriority>();
            var dataModelsNames = Enum.GetNames<Backend.Data.Models.TaskPriority>();

            // Assert
            modelsNames.Should().BeEquivalentTo(dataModelsNames, 
                "Both enums should have the same member names");
        }

        [Theory]
        [InlineData("Low")]
        [InlineData("Medium")]
        [InlineData("High")]
        [InlineData("Critical")]
        public void TaskPriority_StringValueExistsInBothEnums(string priorityName)
        {
            // Assert
            Enum.TryParse<Backend.Models.TaskPriority>(priorityName, out var modelsPriority)
                .Should().BeTrue($"Models.TaskPriority should have a value named {priorityName}");

            Enum.TryParse<Backend.Data.Models.TaskPriority>(priorityName, out var dataModelsPriority)
                .Should().BeTrue($"Data.Models.TaskPriority should have a value named {priorityName}");

            // Both should have the same integer value
            ((int)modelsPriority).Should().Be((int)dataModelsPriority, 
                $"Priority '{priorityName}' should have the same integer value in both enums");
        }

        [Fact]
        public void TaskPriority_RoundTripMapping_PreservesValue()
        {
            // Arrange
            var originalValues = Enum.GetValues<Backend.Models.TaskPriority>();

            foreach (var original in originalValues)
            {
                // Act - Map to Data.Models and back
                var mapped = MappingProfile.MapEnum<Backend.Models.TaskPriority, Backend.Data.Models.TaskPriority>(original);
                var roundTripped = MappingProfile.MapEnum<Backend.Data.Models.TaskPriority, Backend.Models.TaskPriority>(mapped);

                // Assert
                roundTripped.Should().Be(original, $"Round-trip mapping should preserve the value {original}");
            }
        }

        [Fact]
        public void TaskPriority_IntegerValuesMatch()
        {
            // This test ensures that if someone adds a new priority, they add it with matching integer values
            var modelsValues = Enum.GetValues<Backend.Models.TaskPriority>();

            foreach (var modelValue in modelsValues)
            {
                var intValue = (int)modelValue;
                var name = modelValue.ToString();

                // Check if Data.Models has the same integer value
                var dataModelValue = (Backend.Data.Models.TaskPriority)intValue;
                dataModelValue.ToString().Should().Be(name,
                    $"At integer value {intValue}, both enums should have the same name '{name}'");
            }
        }
    }
}
