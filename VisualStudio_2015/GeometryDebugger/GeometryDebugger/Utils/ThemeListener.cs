using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Threading.Tasks;

public class ThemeListener
{

    private bool isUpdated = false;
    private Task currentTask;

    public ThemeListener()
    {
        // Подписка на событие изменения темы
        VSColorTheme.ThemeChanged += OnThemeChanged;
    }

    // Обработчик события, вызываемый при изменении темы
    private void OnThemeChanged(ThemeChangedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("UPDATED");
    }

    public void Unsubscribe()
    {
        // Отписка от события, когда объект больше не нужен
        VSColorTheme.ThemeChanged -= OnThemeChanged;
    }
}
