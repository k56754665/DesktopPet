using UnityEngine;

public struct MonitorBounds
{
    public float Left;
    public float Right;
    public float Top;
    public float Bottom;
}

public static class MonitorUtil
{
    public static MonitorBounds GetBounds(Camera cam)
    {
        MonitorBounds bounds = new MonitorBounds();

        // 픽셀 단위 모니터 크기
        int width = Display.main.systemWidth;
        int height = Display.main.systemHeight;

        // 스크린 좌표를 월드 좌표로 변환
        Vector3 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 topRight   = cam.ScreenToWorldPoint(new Vector3(width, height, cam.nearClipPlane));

        bounds.Left   = bottomLeft.x;
        bounds.Right  = topRight.x;
        bounds.Bottom = bottomLeft.y;
        bounds.Top    = topRight.y;

        return bounds;
    }
}
