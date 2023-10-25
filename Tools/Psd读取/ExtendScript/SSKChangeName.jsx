#target photoshop

// ok and cancel button
var runButtonID = 1;
var cancelButtonID = 2;

///////////////////////////////////////////////////////////////////////////////
// Functions
///////////////////////////////////////////////////////////////////////////////
function ShowMessageBox(message)
{
    var myWindow = new Window ("dialog");
    var myMessage = myWindow.add ("statictext");
    myMessage.text = message;
    myWindow.show();
}

function settingDialog(layerName)
{
    dlgMain = new Window("dialog", "Change Name");
    dlgMain.editText = dlgMain.add("edittext", undefined, layerName);
    dlgMain.btnRun = dlgMain.add("button", undefined, "Confirm");
    dlgMain.btnCancel = dlgMain.add("button", undefined, "Cancel");
    dlgMain.btnRun.onClick = function(){
        dlgMain.close(runButtonID);
    }
    dlgMain.btnCancel.onClick = function(){
        dlgMain.close(cancelButtonID);
    }
	
	var result = dlgMain.show();
    if(runButtonID == result)
    {
        return dlgMain.editText.text;
    }
    else
    {
        return layerName;
    }
}

function main()
{
	try
    {
        var activeLayer = app.activeDocument.activeLayer;
        if(activeLayer != undefined)
        {
            activeLayer.name = settingDialog(activeLayer.name);
        }
	}
	catch(e)
    {
        //if ( DialogModes.NO != app.playbackDisplayDialogs )
        //{
            alert(e);
        //}
    	return 'cancel'; // quit, returning 'cancel' (dont localize) makes the actions palette not record our script
	}
}

///////////////////////////////////////////////////////////////////////////////
// Dispatch
///////////////////////////////////////////////////////////////////////////////

main();