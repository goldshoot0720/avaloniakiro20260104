using avaloniakiro20260104.Models;
using avaloniakiro20260104.ViewModels;

namespace avaloniakiro20260104.Tests;

public class MainWindowViewModelTests
{
    [Fact]
    public async Task MainWindowViewModel_InitializesWithSampleData()
    {
        // Arrange & Act
        var viewModel = new MainWindowViewModel();
        
        // Wait for data loading to complete
        await Task.Delay(100); // Give some time for async loading
        
        // If still loading, wait a bit more
        var maxWait = 5000; // 5 seconds max
        var waited = 0;
        while (viewModel.IsLoading && waited < maxWait)
        {
            await Task.Delay(100);
            waited += 100;
        }

        // Assert
        Assert.NotEmpty(viewModel.FoodItems);
        Assert.NotEmpty(viewModel.Subscriptions);
        Assert.NotEmpty(viewModel.Videos);
        Assert.Equal("儀表板", viewModel.SelectedMenuItem);
    }

    [Fact]
    public async Task TotalFoodItems_ReturnsCorrectCount()
    {
        // Arrange
        var viewModel = new MainWindowViewModel();
        
        // Wait for data loading to complete
        await Task.Delay(100);
        var maxWait = 5000;
        var waited = 0;
        while (viewModel.IsLoading && waited < maxWait)
        {
            await Task.Delay(100);
            waited += 100;
        }

        // Act
        var totalItems = viewModel.TotalFoodItems;

        // Assert
        Assert.Equal(viewModel.FoodItems.Count, totalItems);
    }

    [Fact]
    public async Task MonthlyTotal_CalculatesCorrectSum()
    {
        // Arrange
        var viewModel = new MainWindowViewModel();
        
        // Wait for data loading to complete
        await Task.Delay(100);
        var maxWait = 5000;
        var waited = 0;
        while (viewModel.IsLoading && waited < maxWait)
        {
            await Task.Delay(100);
            waited += 100;
        }

        // Act
        var monthlyTotal = viewModel.MonthlyTotal;

        // Assert
        var expectedTotal = viewModel.Subscriptions.Sum(s => s.Amount);
        Assert.Equal(expectedTotal, monthlyTotal);
    }

    [Fact]
    public void SelectMenuItemCommand_ChangesSelectedMenuItem()
    {
        // Arrange
        var viewModel = new MainWindowViewModel();
        var newMenuItem = "食品管理";

        // Act
        viewModel.SelectMenuItemCommand.Execute(newMenuItem);

        // Assert
        Assert.Equal(newMenuItem, viewModel.SelectedMenuItem);
    }
}

public class FoodItemTests
{
    [Fact]
    public void FoodItem_Status_ReturnsExpiredForPastDate()
    {
        // Arrange
        var foodItem = new FoodItem
        {
            Name = "Test Food",
            ToDate = DateTime.Now.AddDays(-1)
        };

        // Act
        var status = foodItem.Status;

        // Assert
        Assert.Equal("已過期", status);
    }

    [Fact]
    public void FoodItem_Status_ReturnsExpiringForNearFutureDate()
    {
        // Arrange
        var foodItem = new FoodItem
        {
            Name = "Test Food",
            ToDate = DateTime.Now.AddDays(3)
        };

        // Act
        var status = foodItem.Status;

        // Assert
        Assert.Equal("即將過期", status);
    }

    [Fact]
    public void FoodItem_Status_ReturnsNormalForFutureDate()
    {
        // Arrange
        var foodItem = new FoodItem
        {
            Name = "Test Food",
            ToDate = DateTime.Now.AddDays(10)
        };

        // Act
        var status = foodItem.Status;

        // Assert
        Assert.Equal("正常", status);
    }
}

public class StringEqualsConverterTests
{
    [Fact]
    public void Convert_ReturnsTrueForEqualStrings()
    {
        // Arrange
        var converter = StringEqualsConverter.Instance;
        var value = "test";
        var parameter = "test";

        // Act
        var result = converter.Convert(value, typeof(bool), parameter, null);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Convert_ReturnsFalseForDifferentStrings()
    {
        // Arrange
        var converter = StringEqualsConverter.Instance;
        var value = "test1";
        var parameter = "test2";

        // Act
        var result = converter.Convert(value, typeof(bool), parameter, null);

        // Assert
        Assert.False((bool)result);
    }
}