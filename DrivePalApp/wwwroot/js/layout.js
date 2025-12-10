// 这个函数初始化一个监听器，用于将窗口大小的变化报告给Blazor
function initializeLayoutListener(dotnetHelper) {
    // 当窗口大小改变时，调用Blazor的 C# 方法
    window.addEventListener('resize', () => {
        dotnetHelper.invokeMethodAsync('OnWindowResized', window.innerWidth);
    });

    // 初始加载时也调用一次，以设置正确的初始状态
    dotnetHelper.invokeMethodAsync('OnWindowResized', window.innerWidth);
}

// 这个函数负责计算和移动滑动条 
function updateSlider(containerId) {
    const container = document.getElementById(containerId);
    if (!container) return;

    const activeTab = container.querySelector('.nav-link.active');
    const slider = container.querySelector('.nav-slider');

    if (activeTab && slider) {
        const left = activeTab.offsetLeft;
        const width = activeTab.offsetWidth;

        slider.style.width = width + 'px';
        slider.style.left = left + 'px';
    }
}