// C#
// 한글의 수식을 클릭한 채로 GetValue 라벨박스를 클릭하면 그 수식에 맞는 그래프를 변환해줍니다.
// namespace HangulGraphAddIn
// public partial class Form1 : Form
        
private void GetValue_Click(object sender, EventArgs e)
{
    string EQ = null;
    
    // 클릭한 수식 스크립트 화면을 띄우는 액션입니다.
    HWPCONTROLLib.DHwpAction act 
        = (HWPCONTROLLib.DHwpAction)axHwpCtrl1.CreateAction("EquationModify");
    HWPCONTROLLib.DHwpParameterSet pset
        = (HWPCONTROLLib.DHwpParameterSet)act.CreateSet();
    act.GetDefault(pset);
    act.Execute(pset);

    try
    {
        EQ = (string)pset.Item("String");
        if (EQ != null)
        {
            // ScriptEquation 객체를 정의하여 가져온 수식 스크립트를 연산식에 맞는 트리로 변환시켜줍니다.
            ScriptEquation scriptEq = new ScriptEquation(EQ);
            scriptEq.ConvertTree();

            const int numPt = 201;
            List<double> FxList = scriptEq.GetOutputList(numPt);

            // 변환한 트리의 x값에 값을 넣어 곡선을 그립니다.
            HWPCONTROLLib.DHwpAction DrawObjCreatorCurve = (HWPCONTROLLib.DHwpAction)axHwpCtrl1.CreateAction("DrawObjCreatorCurve");
            HWPCONTROLLib.DHwpParameterSet ShapeObject = (HWPCONTROLLib.DHwpParameterSet)DrawObjCreatorCurve.CreateSet();
            HWPCONTROLLib.DHwpParameterSet drawLayOut = (HWPCONTROLLib.DHwpParameterSet)ShapeObject.CreateItemSet("ShapeDrawLayOut", "DrawLayOut");
            HWPCONTROLLib.DHwpParameterSet drawLineAttr = (HWPCONTROLLib.DHwpParameterSet)ShapeObject.CreateItemSet("ShapeDrawLineAttr", "DrawLineAttr");
            HWPCONTROLLib.DHwpParameterSet drawScAction = (HWPCONTROLLib.DHwpParameterSet)ShapeObject.CreateItemSet("ShapeDrawScAction", "DrawScAction");
            DrawObjCreatorCurve.GetDefault(ShapeObject);

            // 1inch = 7200HwpUnit, 기본:1inch
            ShapeObject.SetItem("WidthRelTo", 4);
            ShapeObject.SetItem("Width", 14400);
            ShapeObject.SetItem("HeightRelTo", 2);
            ShapeObject.SetItem("Height", 14400);

            drawLayOut.SetItem("CreateNumPt", numPt);
            HWPCONTROLLib.DHwpParameterArray createPt = (HWPCONTROLLib.DHwpParameterArray)drawLayOut.CreateItemArray("CreatePt", numPt * 2);
            HWPCONTROLLib.DHwpParameterArray curveSegmentInfo = (HWPCONTROLLib.DHwpParameterArray)drawLayOut.CreateItemArray("CurveSegmentInfo", numPt - 1);

            for (int createPt_i = 0; createPt_i < numPt; ++createPt_i)
            {
                double RelValue = FxList[createPt_i] - FxList[0];
                createPt.SetItem(2 * createPt_i,     createPt_i * 100000);
                createPt.SetItem(2 * createPt_i + 1, (int)Math.Round(RelValue * 100000));
            }

            for (int curveSegmentInfo_i = 0; curveSegmentInfo_i < numPt - 1; ++curveSegmentInfo_i)
                curveSegmentInfo.SetItem(curveSegmentInfo_i, 1);

            drawLineAttr.SetItem("Style", 1);
            drawScAction.SetItem("VertFlip", 1);

            DrawObjCreatorCurve.Execute(ShapeObject);
        }
    }
    catch(Exception ex)
    {
        MessageBox.Show(ex.Message);
    }

}
