﻿

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using GoatAutoCAD;
using GoatAutoCAD.baseutil;
using GoatAutoCAD.constant;
using GoatAutoCAD.interaction;

[assembly: CommandClass(typeof(GoatColor))]
namespace GoatAutoCAD{

    public class GoatColor{

        [CommandMethod("MyGroup", "color2", "color2", CommandFlags.Modal)]
        public void color2(){
            ObjectId objectId = InteractionUtil.getEntityId("\n 选择要更改颜色的对象");
            objectId.QOpenForWrite(1,Constant.entityColor);
        }

    }

}
