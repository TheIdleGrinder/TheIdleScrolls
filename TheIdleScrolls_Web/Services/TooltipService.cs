using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace TheIdleScrolls_Web.Services;

public class TooltipService : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;
    private System.Threading.Timer? _hideTimer;
    private int _activeTooltipId = 0;
    private int _currentTooltipId = 0;
    private readonly object _lock = new object();
    private bool _containerActive = true;
    
    public event Action<TooltipRequest?>? OnTooltipRequested;
    
    public TooltipService(IJSRuntime js)
    {
        _js = js;
    }
    
    public void SetContainerActive(bool active)
    {
        lock (_lock)
        {
            _containerActive = active;
            
            if (!active)
            {
                _hideTimer?.Dispose();
                _hideTimer = null;
                _activeTooltipId = 0;
                _currentTooltipId = 0;
                OnTooltipRequested?.Invoke(null);
            }
        }
    }
    
    public async Task ShowAsync(RenderFragment content, ElementReference element)
    {
        bool shouldShow;
        
        lock (_lock)
        {
            if (!_containerActive)
            {
                return;
            }
            
            _hideTimer?.Dispose();
            _hideTimer = null;
            _activeTooltipId = ++_currentTooltipId;
            shouldShow = true;
        }
        
        if (shouldShow)
        {
            try
            {
                await EnsureModuleAsync();
                var rect = await _module!.InvokeAsync<BoundingRect>("getElementPosition", element);
                
                lock (_lock)
                {
                    if (_containerActive && _currentTooltipId == _activeTooltipId)
                    {
                        OnTooltipRequested?.Invoke(new TooltipRequest(content, rect));
                    }
                }
            }
            catch (Exception ex)
            {
                // Silently fail - tooltip is non-critical
                System.Diagnostics.Debug.WriteLine($"Tooltip error: {ex.Message}");
            }
        }
    }
    
    public void Hide()
    {
        int tooltipIdToHide;
        
        lock (_lock)
        {
            tooltipIdToHide = _activeTooltipId;
            
            _hideTimer?.Dispose();
            
            _hideTimer = new System.Threading.Timer(_ => 
            {
                lock (_lock)
                {
                    // Only hide if no new tooltip was shown in the meantime
                    if (tooltipIdToHide == _activeTooltipId)
                    {
                        OnTooltipRequested?.Invoke(null);
                        _activeTooltipId = 0;
                    }
                }
            }, null, 100, Timeout.Infinite);
        }
    }
    
    public void HideImmediate()
    {
        lock (_lock)
        {
            _hideTimer?.Dispose();
            _hideTimer = null;
            _activeTooltipId = 0;
            _currentTooltipId = 0;
            
            OnTooltipRequested?.Invoke(null);
        }
    }

    public async Task<TooltipPosition?> CalculatePositionAsync(BoundingRect elementRect, ElementReference tooltipElement)
    {
        try
        {
            await EnsureModuleAsync();
            var tooltipRect = await _module!.InvokeAsync<BoundingRect>("getElementPosition", tooltipElement);
            
            var position = await _module!.InvokeAsync<TooltipPosition>(
                "calculateTooltipPosition", 
                elementRect, 
                tooltipRect
            );
            
            return position;
        }
        catch
        {
            return null;
        }
    }
    
    private async Task EnsureModuleAsync()
    {
        _module ??= await _js.InvokeAsync<IJSObjectReference>("import", "./js/tooltip.js");
    }
    
    public async ValueTask DisposeAsync()
    {
        lock (_lock)
        {
            _hideTimer?.Dispose();
        }
        
        if (_module != null)
        {
            await _module.DisposeAsync();
        }
    }
}

public record TooltipRequest(RenderFragment Content, BoundingRect Position);

public class BoundingRect
{
    public double Left { get; set; }
    public double Top { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Right { get; set; }
    public double Bottom { get; set; }
    public double ViewportWidth { get; set; }
    public double ViewportHeight { get; set; }
}

public class TooltipPosition
{
    public double Left { get; set; }
    public double Top { get; set; }
    public string Transform { get; set; } = "";
    public double MarginTop { get; set; }
}