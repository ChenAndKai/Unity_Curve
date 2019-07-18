using UnityEditor;
using UnityEngine;
using static BezierSpline;

[CustomEditor(typeof(BezierSpline))]
public class BezierCurveInspector : Editor
{
    private const float directionScale = 0.5f;    //控制每条速度线的长度
    private const int stepsPerCurve = 10;         //控制每条曲线上速度线的数量

    private const float handleSize = 0.04f;        //默认控制点的Size
    private const float pickSize = 0.06f;          //选中控制点的Size
    private int selectIndex = -1;                  //选中控制点的索引,默认为-1，表示没有选中任何控制点


    private BezierSpline spline;                    //BezierSpline类
    private Transform handleTransform;
    private Quaternion handleRotation;

    [System.Obsolete]
    private void OnSceneGUI()
    {
        spline = target as BezierSpline;
        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        for (int i = 1; i < spline.ControlPointCout; i += 3)
        {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 5f);
            p0 = p3;
        }

        ShowDirections();

    }

    /// <summary>
    /// OnInspectorGUI()方法重写，自定义inspector
    /// </summary>
    public override void OnInspectorGUI()
    {


        spline = target as BezierSpline;

        EditorGUI.BeginChangeCheck();
        //EditorGUILayout.Toggle():开关按钮
        bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Toggle Loop");
            EditorUtility.SetDirty(spline);
            spline.Loop = loop;
        }

        //绘制默认的检查器,将参数显示在inspertor上
        //DrawDefaultInspector();

        if (selectIndex >= 0 && selectIndex < spline.ControlPointCout)
        {
            DrawSelectedPointInspector();
        }
        //使用GUILayout绘制按钮
        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }

    }

    /// <summary>
    /// 改变控制点
    /// </summary>
    private void DrawSelectedPointInspector()
    {
        GUILayout.Label("Selected Point");

        //在Inspector面板上显示已选择点的信息
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectIndex));
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline.SetControlPoint(selectIndex, point);
        }



        //改变已选择的点的模式
        EditorGUI.BeginChangeCheck();
        //inspector面板上显示Mode属性，可以弹出枚举选择菜单
        //这里可以实时获取当前选择的点的模式(传入点的索引，找出对应的模式索引下的模式)
        BezierControlPointMode mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectIndex));
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Change Point Mode");
            spline.SetControlPointMode(selectIndex, mode);
            EditorUtility.SetDirty(spline);
        }
    }


    /// <summary>
    /// 画出曲线上某些点的切线（速度线）,保证每一条曲线上的速度线数量相同，由BezierSpline.CurveCount来决定绘制几条线
    /// </summary>
    private void ShowDirections()
    {
        Handles.color = Color.green;
        Vector3 point = spline.GetPoint(0f);
        Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
        int Steps = stepsPerCurve * spline.CurveCount;
        for (int i = 1; i <= Steps; i++)
        {
            point = spline.GetPoint(i / (float)Steps);
            Handles.DrawLine(point, point + spline.GetDirection(i / (float)Steps) * directionScale);
        }
    }

    /// <summary>
    /// 结点颜色
    /// </summary>
    private static Color[] modelColors =
    {
        Color.white,
        Color.yellow,
        Color.cyan
    };

    /// <summary>
    /// Handles操作的是世界坐标系下的点，但是绘制的脚本定义的点是局部坐标系，这里进行转换
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    /// 
    [System.Obsolete]
    private Vector3 ShowPoint(int index)
    {
        //默认情况下，所有点都不能被控制，当被点击时变成活动点，并且出现手柄
        Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
        //HandleUtility.GetHandleSize():给予世界坐标系中的任一点的一个固定的屏幕大小
        float size = HandleUtility.GetHandleSize(point);
        //让样条曲线的起始点明显点
        if(index==0)
        {
            size *= 2f;
        }
        //同一种模式的结点颜色一致
        Handles.color = modelColors[(int)spline.GetControlPointMode(index)];
        //当点击了某一个控制点
        if (Handles.Button(point, handleRotation, size * handleSize, size*pickSize, Handles.DotCap))
        {
            selectIndex = index;
            Repaint();              //样条曲线不能改变，发送一个重新绘制的请求，重绘显示在这个Editor的任何inspector（有点想不明白）
        }
        if (selectIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                //将位置改动后的世界坐标系中的点坐标转换成脚本绑定对象定义的点坐标
                spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point)) ;
            }
        }
        return point;
    }
}
