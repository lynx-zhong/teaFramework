#target photoshop

// ok and cancel button
var runButtonID = 1;
var cancelButtonID = 2;
var checkLayerButtonID = 3;
var copyToResourcesButtonID = 4;
var centerLayerButtonID = 5;
var renameButtonID = 6;
var cleanLayerEffectButtonID = 7;
var cleanParamButtonID = 8;
var cleanResourcesButtonID = 9;

var LAYERTYPE_ARTLAYER = 1;
var LAYERTYPE_LAYERSET = 2;

var LAYOUT_LEFTTOP = 1;
var LAYOUT_LEFT = 2;
var LAYOUT_LEFTBOTTOM = 3;
var LAYOUT_TOP = 4;
var LAYOUT_CENTER = 5;
var LAYOUT_BOTTOM = 6;
var LAYOUT_RIGHTTOP = 7;
var LAYOUT_RIGHT = 8;
var LAYOUT_RIGHTBOTTOM = 9;

var COLORID_RED = charIDToTypeID("Rd  ");
var COLORID_YELLOW = charIDToTypeID("Ylw ");
var COLORID_PURPLE = charIDToTypeID("Prp ");
var COLORID_ORANGE = charIDToTypeID("Orng");

var RESOURCE_TYPE_GLOBAL = 0
var RESOURCE_TYPE_SYSTEM = 1
var RESOURCE_TYPE_LOCAL = 2

var DetailImageName = new Array();
DetailImageName[0] = "multilowpoly";
DetailImageName[1] = "diwen_01";
DetailImageName[2] = "diwen02";
DetailImageName[3] = "diwen_03";
DetailImageName[4] = "diwen_04";
DetailImageName[5] = "diwen_05";
DetailImageName[6] = "diwen_11";
DetailImageName[7] = "diwen_12";

var DecoImageName = new Array();
DecoImageName[0] = "diwen_06"
DecoImageName[1] = "diwen_07"
DecoImageName[2] = "diwen_08"
DecoImageName[3] = "diwen_09"
DecoImageName[4] = "diwen_10"

var CleanParamWhiteList = [
    "@hflip",
    "@vflip",
    "@detail",
    "@detailV",
    "@detailT",
    "@scale",
    "@rotate",
    "@arrg",
    "@dirc",
    "@panel"
];
//var hflipParam = getParamFromString(layerName, "@hflip");
            //var vflipParam = getParamFromString(layerName, "@vflip");
            //var detailParam = getParamFromString(layerName, "@detail");
            //var detailVParam = getParamFromString(layerName, "@detailV");
            //var detailTParam = getParamFromString(layerName, "@detailT");
            //var scaleParam = getParamFromString(layerName, "@scale");
            //var rotateParam = getParamFromString(layerName, "@rotate");
            //var arrgParam = getParamFromString(layerName, "@arrg");
            //var dircParam = getParamFromString(layerName, "@dirc");
            //var panelParam = getParamFromString(layerName, "@panel");

function StringBuffer()
{
    this.strings = new Array();
}

StringBuffer.prototype.append = function(str)
{
    this.strings.push(str);
};

StringBuffer.prototype.toString = function()
{
    return this.strings.join("");
};

function FormatNum(Source,Length)
{ 
    var strTemp=""; 
    for(i=1;i<=Length-Source.length;i++)
    { 
        strTemp+="0"; 
    } 
    return strTemp+Source; 
} 

function ClampNum(value, min, max)
{
    if(value < min)
        return min;
    else if(value > max)
        return max;
    else
        return value;
}

///////////////////////////////////////////////////////////////////////////////
// Function: touchUpLayerSelection
// Usage: deal with odd layer selections of no layer selected or multiple layers
// Input: <none> Must have an open document
// Return: <none>
///////////////////////////////////////////////////////////////////////////////
function touchUpLayerSelection() {
	try{ 
		// Select all Layers
		var idselectAllLayers = stringIDToTypeID( "selectAllLayers" );
			var desc252 = new ActionDescriptor();
			var idnull = charIDToTypeID( "null" );
				var ref174 = new ActionReference();
				var idLyr = charIDToTypeID( "Lyr " );
				var idOrdn = charIDToTypeID( "Ordn" );
				var idTrgt = charIDToTypeID( "Trgt" );
				ref174.putEnumerated( idLyr, idOrdn, idTrgt );
			desc252.putReference( idnull, ref174 );
		executeAction( idselectAllLayers, desc252, DialogModes.NO );
		// Select the previous layer
		var idslct = charIDToTypeID( "slct" );
			var desc209 = new ActionDescriptor();
			var idnull = charIDToTypeID( "null" );
				var ref140 = new ActionReference();
				var idLyr = charIDToTypeID( "Lyr " );
				var idOrdn = charIDToTypeID( "Ordn" );
				var idBack = charIDToTypeID( "Back" );
				ref140.putEnumerated( idLyr, idOrdn, idBack );
			desc209.putReference( idnull, ref140 );
			var idMkVs = charIDToTypeID( "MkVs" );
			desc209.putBoolean( idMkVs, false );
		executeAction( idslct, desc209, DialogModes.NO );
	}catch(e) {
		; // do nothing
	}
}

///////////////////////////////////////////////////////////////////////////////
// Function: hasLayerMask
// Usage: see if there is a raster layer mask
// Input: <none> Must have an open document
// Return: true if there is a vector mask
///////////////////////////////////////////////////////////////////////////////
function hasLayerMask(layerName) 
{
	var hasLayerMask = false;
	try 
    {
        var ref = new ActionReference();
        var keyUserMaskEnabled = app.charIDToTypeID( 'UsrM' );
        ref.putProperty( app.charIDToTypeID( 'Prpr' ), keyUserMaskEnabled );
        
        if(!layerName)
        {
            ref.putEnumerated( app.charIDToTypeID( 'Lyr ' ), app.charIDToTypeID( 'Ordn' ), app.charIDToTypeID( 'Trgt' ) );
        }
        else
        {
            ref.putName(charIDToTypeID('Lyr '), layerName);
        }
    
        var desc = executeActionGet( ref );
        if ( desc.hasKey( keyUserMaskEnabled ) )
        {
            hasLayerMask = true;
        }
    }
    catch(e) 
    {
        hasLayerMask = false;
    }
	
    return hasLayerMask;
}

///////////////////////////////////////////////////////////////////////////////
// Function: hasVectorMask
// Usage: see if there is a vector layer mask
// Input: <none> Must have an open document
// Return: true if there is a vector mask
///////////////////////////////////////////////////////////////////////////////
function hasVectorMask()
{
	var hasVectorMask = false;
    try 
    {
        var ref = new ActionReference();
        var keyVectorMaskEnabled = app.stringIDToTypeID( 'vectorMask' );
        var keyKind = app.charIDToTypeID( 'Knd ' );
        ref.putEnumerated( app.charIDToTypeID( 'Path' ), app.charIDToTypeID( 'Ordn' ), keyVectorMaskEnabled );
        var desc = executeActionGet( ref );
        if ( desc.hasKey( keyKind ) )
        {
            var kindValue = desc.getEnumerationValue( keyKind );
            if (kindValue == keyVectorMaskEnabled)
            {
                hasVectorMask = true;
            }
        }
    }
    catch(e) 
    {
        hasVectorMask = false;
    }

	return hasVectorMask;
}

///////////////////////////////////////////////////////////////////////////////
// Function: selectionFromLayerSelf
// Usage: 
// Input: 
// Return:
///////////////////////////////////////////////////////////////////////////////
function selectionFromLayerSelf(layerName)
{
    // =======================================================
var idsetd = charIDToTypeID( "setd" );
    var desc11 = new ActionDescriptor();
    var idnull = charIDToTypeID( "null" );
        var ref9 = new ActionReference();
        var idChnl = charIDToTypeID( "Chnl" );
        var idfsel = charIDToTypeID( "fsel" );
        ref9.putProperty( idChnl, idfsel );
    desc11.putReference( idnull, ref9 );
    var idT = charIDToTypeID( "T   " );
        var ref10 = new ActionReference();
        var idChnl = charIDToTypeID( "Chnl" );
        var idChnl = charIDToTypeID( "Chnl" );
        var idTrsp = charIDToTypeID( "Trsp" );
        ref10.putEnumerated( idChnl, idChnl, idTrsp );
        if(layerName)
        {
            ref10.putName( charIDToTypeID( "Lyr " ), layerName );
        }
        else
        {
            var ref174 = new ActionReference();
            var idLyr = charIDToTypeID( "Lyr " );
            var idOrdn = charIDToTypeID( "Ordn" );
            var idTrgt = charIDToTypeID( "Trgt" );
            ref10.putEnumerated( idLyr, idOrdn, idTrgt );
        }
    desc11.putReference( idT, ref10 );
executeAction( idsetd, desc11, DialogModes.NO );
}



///////////////////////////////////////////////////////////////////////////////
// Function: SelectionFromLayerMask
// Usage: 
// Input: 
// Return:
///////////////////////////////////////////////////////////////////////////////
function selectionFromLayerMask(layerName, isTarget)
{
    var idsetd = charIDToTypeID( "setd" );
    var desc384 = new ActionDescriptor();
    var idnull = charIDToTypeID( "null" );
    var ref213 = new ActionReference();
    var idChnl = charIDToTypeID( "Chnl" );
    var idfsel = charIDToTypeID( "fsel" );
    ref213.putProperty( idChnl, idfsel );
    desc384.putReference( idnull, ref213 );
    var idT = charIDToTypeID( "T   " );
        var ref214 = new ActionReference();
        var idChnl = charIDToTypeID( "Chnl" );
        var idChnl = charIDToTypeID( "Chnl" );
        var idMsk = charIDToTypeID( "Msk " );
        ref214.putEnumerated( idChnl, idChnl, idMsk );
    
        if(!isTarget)
        {
            if(layerName)
            {
                ref214.putName( charIDToTypeID( "Lyr " ), layerName );
            }
        }
        else
        {
            var ref174 = new ActionReference();
            var idLyr = charIDToTypeID( "Lyr " );
            var idOrdn = charIDToTypeID( "Ordn" );
            var idTrgt = charIDToTypeID( "Trgt" );
            ref214.putEnumerated( idLyr, idOrdn, idTrgt );
        }
    desc384.putReference( idT, ref214 );
    executeAction( idsetd, desc384, DialogModes.NO );
}

function getGlobalLightAngle()
{
    var gaglRef = new ActionReference();
    gaglRef.putEnumerated(charIDToTypeID( "Lyr " ), charIDToTypeID( "Ordn" ), charIDToTypeID( "Trgt" ));
    ang = executeActionGet(gaglRef).getInteger(charIDToTypeID("gblA"));
    
    return ang;
}

function getDropShadowEffect()
{
    var ref = new ActionReference();
    var keyLayerEffects = app.charIDToTypeID( "Lefx" );
    ref.putProperty( app.charIDToTypeID( "Prpr" ), keyLayerEffects );
    ref.putEnumerated( app.charIDToTypeID( "Lyr " ), app.charIDToTypeID( "Ordn" ), app.charIDToTypeID( "Trgt" ) );
    var desc = executeActionGet( ref );
    
    var ang = 0;
    var dstn = 0;
    var opct = 100;
    var r=0,g=0,b=0;
    if(desc.hasKey(keyLayerEffects))
    {
        var descLefx = desc.getObjectValue(keyLayerEffects);
        var idDrSh = charIDToTypeID( "DrSh" )
        if(descLefx.hasKey(idDrSh))
        {
            var descDrSh = descLefx.getObjectValue(idDrSh);
            if(descDrSh.getBoolean(charIDToTypeID("enab")))
            {
                var useGlobalLight = false;
                var iduglg = charIDToTypeID( "uglg" );
                if(descDrSh.hasKey(iduglg))
                {
                    useGlobalLight = descDrSh.getBoolean(iduglg);
                }
                
                if(useGlobalLight)
                {
                    ang = getGlobalLightAngle();
                }
                else
                {
                    var idlagl = charIDToTypeID( "lagl" );
                    if(descDrSh.hasKey(idlagl))
                    {
                        ang = descDrSh.getUnitDoubleValue(idlagl);
                    }
                }
            
                var idDstn = charIDToTypeID( "Dstn" );
                if(descDrSh.hasKey(idDstn))
                {
                    dstn = descDrSh.getUnitDoubleValue(idDstn);
                }
            
                var idOpct = charIDToTypeID( "Opct" );
                if(descDrSh.hasKey(idOpct))
                {
                    opct = descDrSh.getUnitDoubleValue(idOpct);
                }
            
                var idClr = charIDToTypeID( "Clr " );
                var idRGBC = charIDToTypeID( "RGBC" );
                if(descDrSh.hasKey(idClr))
                {
                    var descClr = descDrSh.getObjectValue(idClr);
                    var idRd = charIDToTypeID( "Rd  " );
                    var idGrn = charIDToTypeID( "Grn " );
                    var idBl = charIDToTypeID( "Bl  " );
                    r = descClr.getDouble(idRd);
                    g = descClr.getDouble(idGrn);
                    b = descClr.getDouble(idBl);
                }
            
                var ret = new Object;
                ret.x = -Math.round(Math.cos(ang / 180.0 * Math.PI) * dstn);
                ret.y = Math.round(Math.sin(ang / 180.0 * Math.PI) * dstn);
                
                ret.color = new Array();
                ret.color[0] = Math.round(r);
                ret.color[1] = Math.round(g);
                ret.color[2] = Math.round(b);
                ret.color[3] = Math.round(opct / 100.0 * 255.0);
                
                return ret;
            }
        }
    }
}

function getFrameFxEffect()
{
    var ref = new ActionReference();
    var keyLayerEffects = app.charIDToTypeID( "Lefx" );
    ref.putProperty( app.charIDToTypeID( "Prpr" ), keyLayerEffects );
    ref.putEnumerated( app.charIDToTypeID( "Lyr " ), app.charIDToTypeID( "Ordn" ), app.charIDToTypeID( "Trgt" ) );
    var desc = executeActionGet( ref );
    
    var size = 1;
    var opct = 100;
    var r=0,g=0,b=0;
    if(desc.hasKey(keyLayerEffects))
    {
        var descLefx = desc.getObjectValue(keyLayerEffects);
        var idFrFX = charIDToTypeID( "FrFX" )
        if(descLefx.hasKey(idFrFX))
        {
            var descFrFx = descLefx.getObjectValue(idFrFX);
            if(descFrFx.getBoolean(charIDToTypeID("enab")))
            {
                var idSz = charIDToTypeID("Sz  ");
                if(descFrFx.hasKey(idSz))
                {
                    size = descFrFx.getUnitDoubleValue(idSz);
                }
                
                var idOpct = charIDToTypeID( "Opct" );
                if(descFrFx.hasKey(idOpct))
                {
                    opct = descFrFx.getUnitDoubleValue(idOpct);
                }
                
                var idClr = charIDToTypeID( "Clr " );
                var idRGBC = charIDToTypeID( "RGBC" );
                if(descFrFx.hasKey(idClr))
                {
                    var descClr = descFrFx.getObjectValue(idClr);
                    var idRd = charIDToTypeID( "Rd  " );
                    var idGrn = charIDToTypeID( "Grn " );
                    var idBl = charIDToTypeID( "Bl  " );
                    r = descClr.getDouble(idRd);
                    g = descClr.getDouble(idGrn);
                    b = descClr.getDouble(idBl);
                }
            
                var ret = new Object;
                ret.size = size;
                
                ret.color = new Array();
                ret.color[0] = Math.round(r);
                ret.color[1] = Math.round(g);
                ret.color[2] = Math.round(b);
                ret.color[3] = Math.round(opct / 100.0 * 255.0);
                
                return ret;
            }
        }
    }
}

function getOuterGlowEffect()
{
    var ref = new ActionReference();
    var keyLayerEffects = app.charIDToTypeID( "Lefx" );
    ref.putProperty( app.charIDToTypeID( "Prpr" ), keyLayerEffects );
    ref.putEnumerated( app.charIDToTypeID( "Lyr " ), app.charIDToTypeID( "Ordn" ), app.charIDToTypeID( "Trgt" ) );
    var desc = executeActionGet( ref );
    
    var size = 1;
    var opct = 100;
    var r=0,g=0,b=0;
    if(desc.hasKey(keyLayerEffects))
    {
        var descLefx = desc.getObjectValue(keyLayerEffects);
        var idOrGl = charIDToTypeID( "OrGl" )
        if(descLefx.hasKey(idOrGl))
        {
            var descOrGl = descLefx.getObjectValue(idOrGl);
            if(descOrGl.getBoolean(charIDToTypeID("enab")))
            {
                var idCkmt = charIDToTypeID("Ckmt");
                if(descOrGl.hasKey(idCkmt))
                {
                    size = descOrGl.getUnitDoubleValue(idCkmt);
                }
                
                var idOpct = charIDToTypeID( "Opct" );
                if(descOrGl.hasKey(idOpct))
                {
                    opct = descOrGl.getUnitDoubleValue(idOpct);
                }
                
                var idClr = charIDToTypeID( "Clr " );
                var idRGBC = charIDToTypeID( "RGBC" );
                if(descOrGl.hasKey(idClr))
                {
                    var descClr = descOrGl.getObjectValue(idClr);
                    var idRd = charIDToTypeID( "Rd  " );
                    var idGrn = charIDToTypeID( "Grn " );
                    var idBl = charIDToTypeID( "Bl  " );
                    r = descClr.getDouble(idRd);
                    g = descClr.getDouble(idGrn);
                    b = descClr.getDouble(idBl);
                }
            
                var ret = new Object;
                ret.size = size / 100.0;
                
                ret.color = new Array();
                ret.color[0] = Math.round(r);
                ret.color[1] = Math.round(g);
                ret.color[2] = Math.round(b);
                //ret.color[3] = Math.round(opct / 100.0 * 255.0);
                ret.color[3] = 170;
                
                return ret;
            }
        }
    }
}

function getSolidFill()
{
    var ref = new ActionReference();
    var keyLayerEffects = app.charIDToTypeID( "Lefx" );
    ref.putProperty( app.charIDToTypeID( "Prpr" ), keyLayerEffects );
    ref.putEnumerated( app.charIDToTypeID( "Lyr " ), app.charIDToTypeID( "Ordn" ), app.charIDToTypeID( "Trgt" ) );
    var desc = executeActionGet( ref );
    
    if(desc.hasKey(keyLayerEffects))
    {
        var descLefx = desc.getObjectValue(keyLayerEffects);
        var idSoFi = charIDToTypeID("SoFi")
        if(descLefx.hasKey(idSoFi))
        {
            var descSoFi = descLefx.getObjectValue(idSoFi);
            if(descSoFi.getBoolean(charIDToTypeID("enab")))
            {
                var color = new Object();
                
                var idClr = charIDToTypeID( "Clr " );
                if(descSoFi.hasKey(idClr))
                {
                    var descClr = descSoFi.getObjectValue(idClr);
                    var idRd = charIDToTypeID( "Rd  " );
                    var idGrn = charIDToTypeID( "Grn " );
                    var idBl = charIDToTypeID( "Bl  " );
                    color.r = descClr.getDouble(idRd);
                    color.g = descClr.getDouble(idGrn);
                    color.b = descClr.getDouble(idBl);
                }
            
                var idOpct = charIDToTypeID( "Opct" );  
                if(descSoFi.hasKey(idOpct))
                {
                    color.a = descSoFi.getUnitDoubleValue(idOpct);
                }
                else
                {
                    color.a = 100;
                }
            
                return color;
            }
        }
    }
}

function getGradientFill()
{
    var ref = new ActionReference();
    var keyLayerEffects = app.charIDToTypeID( "Lefx" );
    ref.putProperty( app.charIDToTypeID( "Prpr" ), keyLayerEffects );
    ref.putEnumerated( app.charIDToTypeID( "Lyr " ), app.charIDToTypeID( "Ordn" ), app.charIDToTypeID( "Trgt" ) );
    var desc = executeActionGet( ref );
    
    if(desc.hasKey(keyLayerEffects))
    {
        var descLefx = desc.getObjectValue(keyLayerEffects);
        var idGrFl = charIDToTypeID( "GrFl" )
        if(descLefx.hasKey(idGrFl))
        {
            var descGrFl = descLefx.getObjectValue(idGrFl);
            if(descGrFl.getBoolean(charIDToTypeID("enab")))
            {
                var colorArray = new Array();
                var opctArray = new Array();
                var hasBlend = false;
                var hasScreen = false;
                var isUp = false;
                var Angle = 0;
                
                var idGrad = charIDToTypeID( "Grad" );

                if(descGrFl.hasKey(idGrad))
                {
                    var descGrad = descGrFl.getObjectValue(idGrad);
                    
                    var idClrs = charIDToTypeID( "Clrs" );
                    var idClr = charIDToTypeID( "Clr " );
                    if(descGrad.hasKey(idClrs))
                    {
                        var listClrs = descGrad.getList(idClrs);                        
                        for(var i=0; i<listClrs.count; i++)
                        {
                            var descClr = listClrs.getObjectValue(i).getObjectValue(idClr);
                            if(descClr != undefined)
                            {
                                var color = new Object();
                                var idRd = charIDToTypeID( "Rd  " );
                                var idGrn = charIDToTypeID( "Grn " );
                                var idBl = charIDToTypeID( "Bl  " );
                                color.r = descClr.getDouble(idRd);
                                color.g = descClr.getDouble(idGrn);
                                color.b = descClr.getDouble(idBl);
                                colorArray[i] = color;
                            }
                        }
                    }
                
                    var idTrns = charIDToTypeID( "Trns" );
                    var idOpct = charIDToTypeID( "Opct" );  
                    if(descGrad.hasKey(idTrns))
                    {
                        var listTrns = descGrad.getList(idTrns);
                        for(var i=0; i<listTrns.count; i++)
                        {
                            var descOpct = listTrns.getObjectValue(i);
                            if(descOpct != undefined)
                            {                            
                                var opct = descOpct.getUnitDoubleValue(idOpct);
                                opctArray[i] = opct;
                            }
                        }
                    }
                }
            
                var idMd = charIDToTypeID( "Md  " );
                var idBlnM = charIDToTypeID( "BlnM" );
                var idMltp = charIDToTypeID( "Mltp" );
                var idScrn = charIDToTypeID( "Scrn" );
                if(descGrFl.hasKey(idMd))
                {
                    var blendMode = descGrFl.getEnumerationValue(idMd);
                    if(blendMode == idMltp)
                    {
                        hasBlend = true;
                    }
                    if(blendMode == idScrn)
                    {
                        hasScreen = true;
                    }
                }
            
                var idAngl = charIDToTypeID( "Angl" );
                if(descGrFl.hasKey(idAngl))
                {
                    var ang = descGrFl.getUnitDoubleValue(idAngl);
                    Angle = ang;
                    if(ang > 0)
                    {
                        isUp = true;
                    }
                    else
                    {
                        isUp = false;
                    }
                }
            
                var ret = new Object();
                ret.colorArray = colorArray;
                ret.hasBlend = hasBlend;
                ret.isUp = isUp;
                ret.Angle = Angle;
                ret.hasScreen = hasScreen;
                return ret;
            }
        }
    }
}

///////////////////////////////////////////////////////////////////////////////
// Function: getAllLayerEffectKeys
// Usage: 获取所有图层效果
// Input: isEnabled, true获取打开的效果，false获取关闭的效果, undefined获取所有效果
// Return: 返回图层效果的key.
///////////////////////////////////////////////////////////////////////////////
function getAllLayerEffectKeys(isEnabled, layerName)
{
    var retArray = new Array();
    
    var ref = new ActionReference();
    var keyLayerEffects = app.charIDToTypeID( "Lefx" );
    ref.putProperty( app.charIDToTypeID( "Prpr" ), keyLayerEffects );
    if(layerName != undefined)
    {
        ref.putName(charIDToTypeID( "Lyr " ), layerName)
    }
    else
    {
        ref.putEnumerated( app.charIDToTypeID( "Lyr " ), app.charIDToTypeID( "Ordn" ), app.charIDToTypeID( "Trgt" ) );
    }

    var desc = executeActionGet( ref );    
    if(desc.hasKey(keyLayerEffects))
    {
        var keyEnable = charIDToTypeID("enab");
        var descLefx = desc.getObjectValue(keyLayerEffects);
        for(var i=0; i<descLefx.count; ++i)
        {
            var key = descLefx.getKey(i);
            var type = descLefx.getType(key);
            if(type == DescValueType.OBJECTTYPE)
            {
                var descFx = descLefx.getObjectValue(key);
                if(isEnabled == undefined || (descFx.hasKey(keyEnable) && descFx.getBoolean(keyEnable) == isEnabled))
                {
                    retArray.push(key);
                }
            }
        }
    }
    
    return retArray;
}

///////////////////////////////////////////////////////////////////////////////
// Function: deleteLayerEffect
// Usage: 删除图层效果
// Input: 图层效果的key数组
// Return: 
///////////////////////////////////////////////////////////////////////////////
function deleteLayerEffect(idEffect)
{
    if(idEffect != undefined)
    {
        var iddsfx = charIDToTypeID( "dsfx" );
        var desc72 = new ActionDescriptor();
        var idnull = charIDToTypeID( "null" );
            var ref65 = new ActionReference();
            ref65.putClass( idEffect );
            var idLyr = charIDToTypeID( "Lyr " );
            var idOrdn = charIDToTypeID( "Ordn" );
            var idTrgt = charIDToTypeID( "Trgt" );
            ref65.putEnumerated( idLyr, idOrdn, idTrgt );
        desc72.putReference( idnull, ref65 );
        executeAction( iddsfx, desc72, DialogModes.NO );
    }
}

function getLayerColorFlag()
{
    var ref = new ActionReference();
    ref.putEnumerated(charIDToTypeID( "Lyr " ), charIDToTypeID( "Ordn" ), charIDToTypeID( "Trgt" ));
    var desc = executeActionGet(ref);
    
    var idClr = charIDToTypeID( "Clr " );
    if(desc.hasKey(idClr))
    {
        var enumClr = desc.getEnumerationValue(idClr);
        return enumClr;
    }
}

function setLayerColorFlag(layerName, setColor)
{
    var idslct = charIDToTypeID( "slct" );
    var desc930 = new ActionDescriptor();
    var idnull = charIDToTypeID( "null" );
        var ref717 = new ActionReference();
        var idLyr = charIDToTypeID( "Lyr " );
        ref717.putName( idLyr, layerName );
    desc930.putReference( idnull, ref717 );
    var idMkVs = charIDToTypeID( "MkVs" );
    desc930.putBoolean( idMkVs, false );
    executeAction( idslct, desc930, DialogModes.NO );
    
    var idSetColor = 0;
    if(setColor)
    {
        idSetColor = setColor;
    }
    else
    {
        idSetColor = charIDToTypeID( "null" );
    }
    
    var idsetd = charIDToTypeID( "setd" );
    var desc926 = new ActionDescriptor();
    var idnull = charIDToTypeID( "null" );
        var ref715 = new ActionReference();
        if(layerName != undefined && layerName != "")
        {
            ref715.putName( charIDToTypeID( "Lyr " ), layerName );
        }
        else
        {
            ref715.putEnumerated( charIDToTypeID( "Lyr " ), charIDToTypeID( "Ordn" ), charIDToTypeID( "Trgt" ) );
        }
    desc926.putReference( idnull, ref715 );
    var idT = charIDToTypeID( "T   " );
        var desc927 = new ActionDescriptor();
        var idClr = charIDToTypeID( "Clr " );
        desc927.putEnumerated (idClr, idClr, idSetColor);
    var idLyr = charIDToTypeID( "Lyr " );
    desc926.putObject( idT, idLyr, desc927 );
    executeAction( idsetd, desc926, DialogModes.NO );
}

function getTextLayerRotation()
{
    var ref = new ActionReference()
    ref.putEnumerated(charIDToTypeID( "Lyr " ), charIDToTypeID( "Ordn" ), charIDToTypeID( "Trgt" ));
    var desc = executeActionGet(ref);
    
    var xx = desc.getObjectValue(stringIDToTypeID('textKey'));
    if(xx.hasKey(stringIDToTypeID('transform')))
    {
        xx = xx.getObjectValue(stringIDToTypeID('transform'));
        var yy = xx.getDouble(stringIDToTypeID('yy'));
        var xy = xx.getDouble(stringIDToTypeID('xy'));
    
        var toDegs = 180/Math.PI;
        return Math.atan2(yy, xy) * toDegs - 90;
    }
    return 0;
}

function getTextLayerScale()
{
    var ref = new ActionReference()
    ref.putEnumerated(charIDToTypeID( "Lyr " ), charIDToTypeID( "Ordn" ), charIDToTypeID( "Trgt" ));
    var desc = executeActionGet(ref);
    
    var xx = desc.getObjectValue(stringIDToTypeID('textKey'));
    var textSize = xx.getList(stringIDToTypeID('textStyleRange')).getObjectValue(0).getObjectValue(stringIDToTypeID('textStyle')).getDouble(stringIDToTypeID('size'));
    if(xx.hasKey(stringIDToTypeID('transform')))
    {
        return xx.getObjectValue(stringIDToTypeID('transform')).getUnitDoubleValue(stringIDToTypeID('yy'));
    }
    return 1;
}

function DuplicateLayer(layerName)
{
    var idDplc = charIDToTypeID( "Dplc" );
    var desc967 = new ActionDescriptor();
    var idnull = charIDToTypeID( "null" );
        var ref751 = new ActionReference();
        var idLyr = charIDToTypeID( "Lyr " );
        var idOrdn = charIDToTypeID( "Ordn" );
        var idTrgt = charIDToTypeID( "Trgt" );
        ref751.putEnumerated( idLyr, idOrdn, idTrgt );
    desc967.putReference( idnull, ref751 );
    var idT = charIDToTypeID( "T   " );
        var ref752 = new ActionReference();
        var idDcmn = charIDToTypeID( "Dcmn" );
        ref752.putName( idDcmn, "Resources.psd" );
    desc967.putReference( idT, ref752 );
    var idNm = charIDToTypeID( "Nm  " );
    desc967.putString( idNm, layerName );
    var idVrsn = charIDToTypeID( "Vrsn" );
    desc967.putInteger( idVrsn, 5 );
    executeAction( idDplc, desc967, DialogModes.NO );
}

function MoveLayer(toGroupName)
{
    var idmove = charIDToTypeID( "move" );
    var desc969 = new ActionDescriptor();
    var idnull = charIDToTypeID( "null" );
        var ref754 = new ActionReference();
        var idLyr = charIDToTypeID( "Lyr " );
        var idOrdn = charIDToTypeID( "Ordn" );
        var idTrgt = charIDToTypeID( "Trgt" );
        ref754.putEnumerated( idLyr, idOrdn, idTrgt );
    desc969.putReference( idnull, ref754 );
    var idT = charIDToTypeID( "T   " );
        var ref755 = new ActionReference();
        var idLyr = charIDToTypeID( "Lyr " );
        ref755.putName( idLyr, toGroupName );
    desc969.putReference( idT, ref755 );
    var idAdjs = charIDToTypeID( "Adjs" );
    desc969.putBoolean( idAdjs, false );
    var idVrsn = charIDToTypeID( "Vrsn" );
    desc969.putInteger( idVrsn, 5 );
    executeAction( idmove, desc969, DialogModes.NO );
}

function selectLayer(layerName)
{
    var idslct = charIDToTypeID( "slct" );
    var desc966 = new ActionDescriptor();
    var idnull = charIDToTypeID( "null" );
        var ref750 = new ActionReference();
        var idLyr = charIDToTypeID( "Lyr " );
        ref750.putName( idLyr, layerName );
    desc966.putReference( idnull, ref750 );
    var idMkVs = charIDToTypeID( "MkVs" );
    desc966.putBoolean( idMkVs, false );
    executeAction( idslct, desc966, DialogModes.NO );
}

function makePathFromSelection()
{
    var idMk = charIDToTypeID( "Mk  " );
    var desc453 = new ActionDescriptor();
    var idnull = charIDToTypeID( "null" );
        var ref444 = new ActionReference();
        var idPath = charIDToTypeID( "Path" );
        ref444.putClass( idPath );
    desc453.putReference( idnull, ref444 );
    var idFrom = charIDToTypeID( "From" );
        var ref445 = new ActionReference();
        var idcsel = charIDToTypeID( "csel" );
        var idfsel = charIDToTypeID( "fsel" );
        ref445.putProperty( idcsel, idfsel );
    desc453.putReference( idFrom, ref445 );
    var idTlrn = charIDToTypeID( "Tlrn" );
    var idPxl = charIDToTypeID( "#Pxl" );
    desc453.putUnitDouble( idTlrn, idPxl, 1.000000 );
    executeAction( idMk, desc453, DialogModes.NO );
}

function getPathPoints(pathName)
{
    var retArray = new Array();
    
    var ref = new ActionReference()
    ref.putName(charIDToTypeID("Path"), pathName);
    var desc = executeActionGet(ref);
    
    if(desc)
    {
        var pathComponents = desc.getObjectValue(charIDToTypeID("PthC")).getList(stringIDToTypeID("pathComponents"));
        if(pathComponents && pathComponents.count > 0)
        {
            var subPathList = pathComponents.getObjectValue(0).getList(charIDToTypeID("SbpL"));
            if(subPathList && subPathList.count > 0)
            {
                var points = subPathList.getObjectValue(0).getList(charIDToTypeID("Pts "));
                if(points)
                {
                    for(var i=0; i<points.count; ++i)
                    {
                        var point = points.getObjectValue(i);
                        if(point)
                        {
                            var anchor = point.getObjectValue(charIDToTypeID("Anch"));
                            if(anchor)
                            {
                                var pointObj = new Object();
                                pointObj.x = anchor.getUnitDoubleValue(charIDToTypeID("Hrzn"));
                                pointObj.y = anchor.getUnitDoubleValue(charIDToTypeID("Vrtc"));
                                retArray.push(pointObj);
                            }
                        }
                    }
                }
            }
        }
    }
    
    return retArray;
}

function CollapsePath(pathPoints, expectNum)
{
    if(pathPoints.length <= expectNum)
    {
        return pathPoints;
    }
    
    var newArray = new Array();
    for(var i=0; i<pathPoints.length; ++i)
    {
        var pIndex = ((i - 1) + pathPoints.length) % pathPoints.length;
        var nIndex = (i + 1) % pathPoints.length;
        var pPoint = pathPoints[pIndex];
        var cPoint = pathPoints[i];
        var nPoint = pathPoints[nIndex];
        
        var k1, k2;
        if(cPoint.x != pPoint.x)
        {
            k1 = Math.atan((cPoint.y - pPoint.y) / (cPoint.x - pPoint.x));
        }
        else
        {
            k1 = 90;
        }
        
        if(cPoint.x != nPoint.x)
        {
            k2 = Math.atan((nPoint.y - cPoint.y) / (nPoint.x - cPoint.x));
        }
        else
        {
            k2 = 90;
        }
        
        if(Math.abs(k2 - k1) > 1)
        {
            newArray.push(cPoint);
        }
    }

    while(newArray.length > expectNum)
    {
        var minIndex1 = -1;
        var minIndex2 = -1;
        var minLength2 = Number.MAX_VALUE;
        for(var i=0; i<newArray.length; ++i)
        {
            var index1 = i;
            var index2 = (i + 1) % newArray.length;
            var point1 = newArray[index1];
            var point2 = newArray[index2];
            var len2 = Math.pow(point2.x - point1.x, 2) + Math.pow(point2.y - point1.y, 2);
            if(minLength2 > len2)
            {
                minLength2 = len2;
                minIndex1 = index1;
                minIndex2 = index2;
            }
        }
    
        if(minIndex1 == -1 || minIndex2 == -1)
            break;
            
        var mp0 = newArray[((minIndex1 - 1) + newArray.length) % newArray.length];
        var mp1 = newArray[minIndex1];
        var mp2 = newArray[minIndex2];
        var mp3 = newArray[(minIndex2 + 1) % newArray.length];
        var t1 = mp0.x * (mp3.y - mp2.y) + mp2.x * (mp0.y - mp3.y) + mp3.x * (mp2.y - mp0.y);
        var t2 = -(mp0.x * (mp2.y - mp1.y) + mp1.x * (mp0.y - mp2.y) + mp2.x * (mp1.y - mp0.y));
        var d = mp0.x * (mp3.y - mp2.y) + mp1.x * (mp2.y - mp3.y) + mp3.x * (mp1.y - mp0.y) + mp2.x * (mp0.y - mp1.y);
        if(d != 0)
        {
            var dx = mp1.x - mp0.x;
            var dy = mp1.y - mp0.y;
            var newPoint = new Object();
            newPoint.x = mp0.x + t1 * dx / d;
            newPoint.y = mp0.y + t1 * dy / d;
            if(minIndex1 != newArray.length - 1)
            {
                newArray.splice(minIndex1, 2, newPoint);
            }
            else
            {
                newArray.splice(minIndex1, 1, newPoint);
                newArray.shift();
            }
        }
        else
        {
            newArray.splice(minIndex1, 1);
        }
    }

    return newArray;
}

///////////////////////////////////////////////////////////////////////////////
// Function: LogActionObj
// Usage: Print all content of ActionDescriptor
// Input: <none> 
// Return: <none>
///////////////////////////////////////////////////////////////////////////////
function LogActionObj(desc, indent)
{
    for(var i=0; i<desc.count; ++i)
    {
        var key = desc.getKey(i);
        var type = desc.getType(key);
        
        LogActionValue(desc, key, type, indent, false);
    }
}

function LogActionList(actionList, indent)
{
    for(var i=0; i<actionList.count; ++i)
    {
        var type = actionList.getType(i);
        
        LogActionValue(actionList, i, type, indent, true);
    }
}

function LogActionValue(desc, key, type, indent, isIndex)
{
    indent = indent ? indent : 0;
    for(var i=0; i<indent; ++i)
    {
        $.write("\t");
    }

    var keyString = key;
    isIndex = (isIndex != undefined) ? isIndex : false;
    if(!isIndex)
    {
        keyString = typeIDToStringID(key);
    }
    
    if(type == DescValueType.BOOLEANTYPE)
    {
        var value = desc.getBoolean(key);
        $.writeln(keyString + ", " + type + ", " + value);
    }
    else if(type == DescValueType.DOUBLETYPE)
    {
        var value = desc.getDouble(key);
        $.writeln(keyString + ", " + type + ", " + value);
    }
    else if(type == DescValueType.ENUMERATEDTYPE)
    {
        var enumType = desc.getEnumerationType(key);
        var enumValue = desc.getEnumerationValue(key);
        $.writeln(keyString + ", " + type + ", " + enumType + ", " + enumValue);
    }
    else if(type == DescValueType.INTEGERTYPE)
    {
        var value = desc.getInteger(key);
        $.writeln(keyString + ", " + type + ", " + value);
    }
    else if(type == DescValueType.LARGEINTEGERTYPE)
    {
        var value = desc.getLargeInteger(key);
        $.writeln(keyString + ", " + type + ", " + value);
    }
    else if(type == DescValueType.STRINGTYPE)
    {
        var value = desc.getString(key);
        $.writeln(keyString + ", " + type + ", " + value);
    }
    else if(type == DescValueType.UNITDOUBLE)
    {
        var unitType = desc.getUnitDoubleType(key);
        var unitValue = desc.getUnitDoubleValue(key);
        $.writeln(keyString + ", " + type + ", " + unitType + ", " + unitValue);
    }
    else if(type == DescValueType.LISTTYPE)
    {
        var value = desc.getList(key);
        $.writeln(keyString + ", " + type + ", " + value);
        LogActionList(value, indent + 1);
    }
    else if(type == DescValueType.OBJECTTYPE)
    {
        var objType = desc.getObjectType(key);
        var objValue = desc.getObjectValue(key);
        $.writeln(keyString + ", " + type + ", " + objType + ", " + objValue);
        LogActionObj(objValue, indent + 1);
    }
    else
    {
        $.writeln(keyString + ", " + type);
    }
}


///////////////////////////////////////////////////////////////////////////////
// Dispatch
///////////////////////////////////////////////////////////////////////////////

//获取图层名字
function getLayerName(layer)
{
    var name = layer.name;
    var cmdIndex = name.indexOf("@", 0);
    if(cmdIndex != -1)
    {
        name = name.substr(0, cmdIndex);
    }

    return name;
}

//获取图片名字
function getImageName(layer)
{
    var name = layer.name;
    var cmdIndex = name.indexOf("#", 0);
    if(cmdIndex != -1)
    {
        name = name.substr(0, cmdIndex);
    }
    else
    {
        name = getLayerName(layer);
    }
    
    return name;
}


//检测是否包含非法字符
function testChinaStr(str)
{
    if(/.*[\u4e00-\u9fa5]+.*$/.test(str))
    {
        return false;
    }

    if(str.indexOf(' ') != -1)
    {
        return false;
    }
    

    return true;
    
}

//插入参数
function insertParamToString(str, key, value)
{
    var valueStr = key + value;
    var ret = "";
    if(str.indexOf(key) != -1)
    {
        var pattern = 'key[^@]*([@.*]{0,1})';
        pattern = pattern.replace('key', key);
        var regExp = new RegExp(pattern, 'i');
        ret = str.replace(regExp, valueStr + "$1");
    }
    else
    {
        ret = str + valueStr;
    }
    
    return ret;
}

//移除参数
function removeParamFromString(str, key)
{
    var ret = str;
    if(str.indexOf(key) != -1)
    {
        var pattern = 'key[^@]*';
        pattern = pattern.replace('key', key);
        var regExp = new RegExp(pattern, 'i');
        ret = str.replace(regExp, "");
    }

    return ret;
}

//获取参数
function getParamFromString(str, key)
{
    var ret = "";
    if(str.indexOf(key) != -1)
    {
        var pattern = 'key[^@]*';
        pattern = pattern.replace('key', key);
        var regExp = new RegExp(pattern, 'i');
        ret = str.match(regExp);
    }

    return ret;
}

//是否含有参数
function hasParamFromString(str, key)
{
    if(str.indexOf(key) != -1)
    {
        return true;
    }
    
    return false;
}

main();

///////////////////////////////////////////////////////////////////////////////
// Functions
///////////////////////////////////////////////////////////////////////////////

function settingDialog(exportInfo)
{
	dlgMain = new Window("dialog", "Test Dialog");
	
    dlgMain.btnCheck = dlgMain.add("button", undefined, "检查");
    dlgMain.btnCheck.onClick = function()
    {
        dlgMain.close(checkLayerButtonID);
    }

    dlgMain.btnRename = dlgMain.add("button", undefined, "自动重命名");
    dlgMain.btnRename.onClick = function(){
        dlgMain.close(renameButtonID);
    }

    dlgMain.btnCopyToResources = dlgMain.add("button", undefined, "拷贝资源到Resources");
    dlgMain.btnCopyToResources.onClick = function(){
        try
        {
            isCover = (askDialog("是否覆盖已有的资源?") == runButtonID);
            CopyToResources(isCover);
        }
        catch(e)
        {
            alert(e);
        }
        
        ShowMessageBox("完成!")
    }

    dlgMain.btnCenterLayer = dlgMain.add("button", undefined, "按选中图层居中");
    dlgMain.btnCenterLayer.onClick = function(){
        try
        {
            CenterLayer();
        }
        catch(e)
        {
            alert(e);
        }
    
        ShowMessageBox("完成!")
    }

    dlgMain.btnCleanLayerEffect = dlgMain.add("button", undefined, "清除隐藏的图层样式");
    dlgMain.btnCleanLayerEffect.onClick = function(){
        dlgMain.close(cleanLayerEffectButtonID);
    }

    dlgMain.btnCleanParam = dlgMain.add("button", undefined, "清除参数");
    dlgMain.btnCleanParam.onClick = function(){
        dlgMain.close(cleanParamButtonID);
    }

    dlgMain.btnCleanResources = dlgMain.add("button", undefined, "标记不用的资源");
    dlgMain.btnCleanResources.onClick = function(){
        dlgMain.close(cleanResourcesButtonID);
    }

	dlgMain.btnRun = dlgMain.add("button", undefined, "Run");
	dlgMain.btnRun.onClick = function() {
		dlgMain.close(runButtonID);
	}    
    
	var result = dlgMain.show();
	
	return result;
}

function askDialog(content)
{
    askMain = new Window("dialog", "Warning");
    askMain.add("statictext", undefined, content);
    askMain.btnYes = askMain.add("button", undefined, "是");
    askMain.btnYes.onClick = function(){
        askMain.close(runButtonID);
    }
    askMain.btnNo = askMain.add("button", undefined, "否");
    askMain.btnNo.onClick = function(){
        askMain.close(cancelButtonID);
    }

    var result = askMain.show();
    return result;
}

function WarningDialog(content) 
{
    warningMain = new Window("dialog", "Warning");
    
    var message = warningMain.add("edittext", undefined, "", {multiline:true,readonly:true});
    message.minimumSize.width = 500;
    message.minimumSize.height = 300;
    message.text = content;
    message.active = true;
    warningMain.show();
}

function ShowMessageBox(message)
{
    var myWindow = new Window ("dialog");
    var myMessage = myWindow.add ("statictext");
    myMessage.text = message;
    myWindow.show();
}

function main()
{	
	var exportInfo = new Object();
    
	initExportInfo(exportInfo);
	
    var retButtonID = cancelButtonID;
	//if ( DialogModes.ALL == app.playbackDisplayDialogs )
    {
        retButtonID = settingDialog(exportInfo);
    	if (cancelButtonID == retButtonID) 
        {
	    	return 'cancel'; // quit, returning 'cancel' (dont localize) makes the actions palette not record our script
	    }
	}
	
	try
    {
        if(retButtonID == checkLayerButtonID)
        {
            var globalResources = new Array();
            var collectResRet = CollectResources(globalResources);
            if(collectResRet != true)
            {
                WarningDialog(collectResRet);
                return;
            }

            touchUpLayerSelection();
            
            //cancel all color.
            for(var nl=0; nl<app.activeDocument.layers.length; nl++)
            {
                setLayerColorFlag(app.activeDocument.layers[nl].name, false)
            }
        
            var errorLayers = new Array();
           
            //check local resources.            
            var localResources = new Array();                     
            for(var rl = 0; rl<app.activeDocument.layers.length; rl++)
            {
                if(app.activeDocument.layers[rl].name == "Resources")
                {
                    CollectResourcesRecursive(app.activeDocument.layers[rl], localResources, RESOURCE_TYPE_LOCAL);                    
                    break;
                }
            }

            for(var key in localResources)
            {
                var value = localResources[key];
                if(globalResources[key] == undefined)
                {
                    var errorLayer = new Object();
                    errorLayer.name = value.name;
                    errorLayer.fullName = value.fullName;
                    errorLayer.errors = new Array();
                    errorLayer.errors.push("该资源在Resources.psd中不存在！");
                    errorLayers.push(errorLayer);
                }
            }
        
            //check layer error.       
            var layersCount = new Array();
            var nameSet = new Array();
            CheckLayerError(app.activeDocument, globalResources, localResources, errorLayers, layersCount, nameSet);
            
            if(errorLayers.length > 0)
            {
                for(var elIndex=0; elIndex<errorLayers.length; elIndex++)
                {
                    setLayerColorFlag(errorLayers[elIndex].fullName, COLORID_RED)
                }
                
                var stringBuffer = new StringBuffer();
                for(var i=0; i<errorLayers.length; i++)
                {
                    var errLayer = errorLayers[i];              
                    for(var j=0; j<errLayer.errors.length; j++)
                    {
                        stringBuffer.append(errLayer.name);
                        stringBuffer.append("  ");
                        stringBuffer.append(errLayer.errors[j]);
                        stringBuffer.append("\r\n");
                    }
                }
            
                WarningDialog(stringBuffer.toString());
            }
            else
            {
                WarningDialog("No Error");
            }
            
        }
        else if(retButtonID == renameButtonID)
        {
            var nameSet = new Object();
            var count = 0;
            CheckLayerName(app.activeDocument, nameSet, count);
            
            ShowMessageBox("完成!")
        }
        else if(retButtonID == cleanLayerEffectButtonID)
        {
            touchUpLayerSelection();

            CleanLayerEffect(app.activeDocument);
        }
        else if(retButtonID == cleanParamButtonID)
        {
            touchUpLayerSelection();
            
            CleanLayerParam(app.activeDocument)
        }
        else if(retButtonID == cleanResourcesButtonID)
        {
            CleanNotUsedResources();
            
            ShowMessageBox("完成!")
        }
        else if(retButtonID == runButtonID)
        {
            touchUpLayerSelection();
            
            FillLayerCommand(app.activeDocument);
            
            //移除detail图以后再计算位置，避免父节点偏移太大
            var detailImages = new Array();
            CollectDetailImage (app.activeDocument, detailImages);
            var tempLayerSet = app.activeDocument.layerSets.add();
            for(var i=0; i<detailImages.length; i++)
            {
                var layer = detailImages[i].layer;
                if(layer != undefined)
                {
                    layer.move(tempLayerSet, ElementPlacement.INSIDE);
                }
            }
            
            FillLayerSetCommand(app.activeDocument);
            
            for(var i=0; i<detailImages.length; i++)
            {
                var layer = detailImages[i].layer; 
                if(layer != undefined)
                {   
                    var nextLayer = detailImages[i].nextLayer;
                    var parent = detailImages[i].parent;
                    if(nextLayer != undefined)
                    {
                        layer.move(nextLayer, ElementPlacement.PLACEBEFORE);
                    }
                    else if(parent != undefined)
                    {
                        layer.move(parent, ElementPlacement.PLACEATEND);
                    }
                }
            }
            
            if(tempLayerSet.layers.length > 0)
            {
                alert("错误，未能恢复图层顺序");
            }
            
            tempLayerSet.remove();
            
            app.activeDocument.save();
            
            ShowMessageBox("完成!")
        }
	}
	catch(e)
    {
        //if ( DialogModes.NO != app.playbackDisplayDialogs )
        //{
            if(app.activeDocument.activeLayer)
            {
                alert(app.activeDocument.activeLayer.name + e);
            }
            else
            {
                alert(e);
            }
        //}
    	return 'cancel'; // quit, returning 'cancel' (dont localize) makes the actions palette not record our script
	}
}

function CollectResources(resArray)
{
    var currentDocument = app.activeDocument;
    
    // collect global resources.
    try
    {
        var globalRsPath = app.activeDocument.path + "/../CommonAtlas/Resources.psd";
        var globalRsDoc = app.open(File(globalRsPath));
        if(globalRsDoc != undefined)
        {
            CollectResourcesRecursive(globalRsDoc, resArray, RESOURCE_TYPE_GLOBAL);
        }
        else
        {
            return "Common Atlas 不存在!"
        }
        
        globalRsDoc.close(SaveOptions.DONOTSAVECHANGES);
    }
    catch(e){}
    
    // collect system resources.
    var resDoc = undefined;
    try
    {
        resDoc = app.documents.getByName("Resources.psd");
    }
    catch(e){}
    
    try
    {
        var resourcePath = app.activeDocument.path + "/Resources.psd";
        if(resDoc == undefined || resDoc.fullName != resourcePath)
        {
            var resFile = File(resourcePath);
            resDoc = app.open(resFile);
        }
    }
    catch(e){}

    if(resDoc == undefined)
    {
        return "Resources.psd 不存在!";
    }

	app.activeDocument = resDoc;
	
    touchUpLayerSelection();
    CollectResourcesRecursive(resDoc, resArray, RESOURCE_TYPE_SYSTEM);

    app.activeDocument = currentDocument;

    return true;
}

function CollectResourcesRecursive(obj, resArray, resType)
{
    var layerCount = obj.layers.length
    for(var i=0; i<layerCount; i++)
    {
        var layer = obj.layers[i];
        app.activeDocument.activeLayer = layer;
        
        var layerType = 0;
        if(layer.typename == "ArtLayer")
        {
            if(layer.kind == LayerKind.NORMAL)
            {
                var bounds = layer.bounds;
                var imageName = getImageName(layer);
                var resource = new Object();  
                resource.name = imageName;
                resource.fullName = layer.name;
                resource.width = bounds[2].value - bounds[0].value;
                resource.height = bounds[3].value - bounds[1].value;
                resource.resType = resType;
                
                if(resArray[imageName] != undefined && resArray[imageName].resType == resType)
                {
                    setLayerColorFlag(layer.name, COLORID_RED)
                    alert(imageName + "资源重复!")
                }
                else
                {
                    resArray[imageName] = resource;
                }
            
                if(hasLayerMask())
                {
                    var zeroCount = 0;
                    var sliceParam = getSliceAreaParam(layer, true);
                    for(var z=0; z < sliceParam.length; ++z)
                    {
                        if(sliceParam[z] == 0)
                        {
                            zeroCount = zeroCount + 1;
                        }
                    }
                    
                    if(zeroCount == 2)
                    {
                        if(!((sliceParam[0] > 0 && sliceParam[2] > 0) || (sliceParam[1] > 0 && sliceParam[3] > 0)))
                        {
                            setLayerColorFlag("", COLORID_RED)
                            alert(imageName + "九宫参数错误。");
                        }
                    }
                    else if(zeroCount == 1 || zeroCount == 4)
                    {
                        setLayerColorFlag("", COLORID_RED)
                        alert(imageName + "九宫参数错误。");
                    }
                }
            
                var layerEffects = getAllLayerEffectKeys(undefined);
                if(layerEffects != undefined && layerEffects.length > 0)
                {
                    setLayerColorFlag("", COLORID_RED)
                    alert(imageName + "资源图层不能有图层效果");
                }
            }
        }
        else if(layer.typename == "LayerSet")
        {
            CollectResourcesRecursive(layer, resArray, resType);
        }
    }
}

function IsAllLayerLocked(layerset)
{
    var childCount = layerset.layers.length;
    for(var i=0; i<childCount; i++)
    {
        var layer = layerset.layers[i];
        if(IsNoExportLayer(layer))
        {
            continue;
        }
        
        if(layer.typename == "ArtLayer" && layer.kind != LayerKind.SOLIDFILL)
        {
            return false;
        }
    
        if(layer.typename == "LayerSet")
        {
            if(!IsAllLayerLocked(layer))
            {
                return false;
            }
        }
    }

    return true;
}

function IsDetailImage(name)
{
    var isDetailImage = false;
    for(i=0; i<DetailImageName.length; i++)
    {
        if(name == DetailImageName[i])
        {
            isDetailImage = true;
            break;
        }
    }

    return isDetailImage;
}

function IsDecoImage(name)
{
    var isDecoImage = false;
    for(i=0; i<DecoImageName.length; i++)
    {
        if(name == DecoImageName[i])
        {
            isDecoImage = true;
            break;
        }
    }
    
    return isDecoImage;
}

function CheckLayerSet(layerset)
{
    var layersetName = layerset.name
    if(hasParamFromString(layersetName, "scale") ||
        hasParamFromString(layersetName, "rotate"))
    {
        if(!IsAllLayerLocked(layerset))
        {
            return "图层组上设置了旋转或缩放,但是下面有需要导出的图层.";
        }
    }
    else if(hasParamFromString(layersetName, "grid"))
    {
        var arrangeParam = CalculateArrangeParam(layerset);
        if(arrangeParam.HasUnusualStep)
        {
            return "Grid排布没对齐,请检查ITEM大小是否一致,或者中心点是否对齐.";
        }
    }

    return undefined;
}

function IsNoExportLayer(layer)
{
    if(layer.allLocked)
        return true;
        
    if(layer.typename == "ArtLayer" && layer.kind == LayerKind.SOLIDFILL)
        return true;

    
    if(IsDetailImage(getLayerName(layer)))
    {
        var layerFullName = layer.name;
        if(layerFullName.indexOf("@detail") == -1 && layerFullName.indexOf("@detailV") == -1 && layerFullName.indexOf("@detailT") == -1)
        {
            if(layer.linkedLayers != undefined && layer.linkedLayers.length > 0)
            {
                var linkLayer = layer.linkedLayers[0];
                if(linkLayer.typename == "ArtLayer")
                {
                    var linkLayerFullName = linkLayer.name;
                    if(linkLayerFullName.indexOf("@detail") != -1 || linkLayerFullName.indexOf("@detailV") != -1|| linkLayerFullName.indexOf("@detailT") != -1)
                    {
                        return true;
                    }
                }
            }
        }
    }
    
    return false;
}

function CheckLayerError(obj, globalResources, localResources, errorLayers, layersCount, nameSet)
{
    var layerCount = obj.layers.length;
    for(var i=0; i<layerCount; i++)
    {
        var layer = obj.layers[i];
        app.activeDocument.activeLayer = layer;
        
        var layerType = 0;
        if(layer.typename == "ArtLayer")
        {
            layerType = LAYERTYPE_ARTLAYER;
        }
        else if(layer.typename == "LayerSet")
        {
            layerType = LAYERTYPE_LAYERSET;
        }
        
        if(IsNoExportLayer(layer)) //No Export
        {
            continue;
        }
    
        if(layerType == LAYERTYPE_LAYERSET)
        {
            if(layer.name != "Resources")
            {
                CheckLayerError(layer, globalResources, localResources, errorLayers, layersCount, nameSet);
            } 
        }

        var errorString = new Array();

        //check layer name.        
        var layerName = getLayerName(layer);
        if(testChinaStr(layerName) == false)
        {
            errorString.push("图层名中含有非法字符.");
        }
        
        if(layerType == LAYERTYPE_ARTLAYER)
        {
            if(layer.kind == LayerKind.TEXT)
            {     
                var textItem = layer.textItem
                if(textItem != undefined)
                {
                    var fontName = textItem.font;
                    if(fontName.indexOf("MicrosoftYaHei") == -1 &&
                        fontName.indexOf("MicrosoftYaHei-Bold") == -1 &&
                        fontName.indexOf("FZLTDHJW--GB1-0") == -1 &&
                        fontName.indexOf("FZLTHK--GBK1-0") == -1 &&
                        fontName.indexOf("FZLTDHK--GBK1-0") == -1)
                    {
                        errorString.push("不能识别的字体,将默认使用MainFont");
                    }
                    
                    var isBold = false;
                    try
                    {
                        isBold = textItem.fauxBold;
                    }
                    catch(e){}

                    if(isBold)
                    {
                        errorString.push("不支持粗体");
                    }
                }
            
                if(getSolidFill() != undefined)
                {
                    errorString.push("文本不能使用颜色叠加,请使用文本颜色");
                }
            }
            else if(layer.kind == LayerKind.NORMAL)
            {
                var imageName = getImageName(layer);
                var layerFullName = layer.name
                var isDetailLayer = false;
                var isDetailImage = IsDetailImage(imageName);
                var isDecoImage = IsDecoImage(imageName);
                var hasLM = hasLayerMask();
                
                if(isDetailImage)
                {
                    isDetailLayer = true;
                    if(layerFullName.indexOf("@detail") == -1 && layerFullName.indexOf("@detailV") == -1 && layerFullName.indexOf("@detailT") == -1)
                    {
                        errorString.push("检测到该图层可能为叠加图层，但是没有加@detail参数.");
                    }
                }
            
                if(layerFullName.indexOf("@detail") != -1 || layerFullName.indexOf("@detailV") != -1|| layerFullName.indexOf("@detailT") != -1)
                {
                    isDetailLayer = true;           
                    if(!isDetailImage)
                    { 
                        errorString.push("错误的叠加纹理.");
                    }
                }
                
                //check layer if same as resource layer.
                if(!isDetailLayer && !isDecoImage)
                {
                    if(globalResources[imageName] == undefined &&
                       localResources[imageName] == undefined)
                    {
                        if(hasLM || layersCount[imageName] != undefined)
                        {
                            errorString.push("该图层为复用资源，需要放到Resources中.");
                        }
                    }
                    
                    if(layersCount[imageName] == undefined)
                    {
                        layersCount[imageName] = 1;
                    }
                    else
                    {
                        layersCount[imageName]++;
                    }
                }
  
                if(isDetailLayer)
                {
                    if(layer.blendMode != BlendMode.OVERLAY)
                    {
                        errorString.push("叠加图层必须是叠加模式.");
                    }
                
                    var nextLayer = obj.layers[i+1];
                    if(nextLayer != undefined)
                    {
                        // check detail bound is overlap this layer.
                        var detailBound = new Array();
                        var layerBound;
                        selectionFromLayerSelf();
                        var selection = app.activeDocument.selection;
                        if(selection != undefined)
                        {
                            detailBound[0] = selection.bounds[0];
                            detailBound[1] = selection.bounds[1];
                            detailBound[2] = selection.bounds[2];
                            detailBound[3] = selection.bounds[3];

                            selection.deselect();
                        }
                        layerBound = nextLayer.bounds;
                        if(layerBound[0] < detailBound[0] || layerBound[1] < detailBound[1] || layerBound[2] > detailBound[2] || layerBound[3] > detailBound[3])
                        {
                            errorString.push("底纹图没有完全覆盖被叠加图");
                        }
                        
                        // check gradient mode.
                        app.activeDocument.activeLayer = nextLayer;
                        var gradientParam = getGradientFill();
                        if(gradientParam != undefined)
                        {
                            if(!gradientParam.hasScreen)
                            {
                                errorString.push("被叠加的图层有渐变叠加，但是不是滤色模式");
                            }
                        }
                        app.activeDocument.activeLayer = layer;
                    }
                    else
                    {
                        errorString.push("找不到要叠加的图层");
                    }
                }
                else
                {
                    if(layer.blendMode != BlendMode.NORMAL && layer.blendMode != BlendMode.LINEARDODGE)
                    {
                        errorString.push("不支持的混合模式,只能是'正常'和'线性减淡'.");
                    }
                
                    var gradientParam = getGradientFill();
                    if(gradientParam != undefined)
                    {
                        if(!gradientParam.hasScreen && !gradientParam.hasBlend)
                        {
                            errorString.push("不支持的渐变叠加，必须是滤色或者正片叠底");
                        }
                    }
                
                    if(hasLM)
                    {
                        var zeroCount = 0;
                        var sliceParam = getSliceAreaParam(layer, true);
                        for(var z=0; z < sliceParam.length; ++z)
                        {
                            if(sliceParam[z] == 0)
                            {
                                zeroCount = zeroCount + 1;
                            }
                        }
                        
                        if(zeroCount == 2)
                        {
                            if(!((sliceParam[0] > 0 && sliceParam[2] > 0) || (sliceParam[1] > 0 && sliceParam[3] > 0)))
                            {
                                errorString.push("九宫参数错误。");
                            }
                        }
                        else if(zeroCount == 1 || zeroCount == 4)
                        {
                            errorString.push("九宫参数错误。");
                        }
                    }
                }
            
                if(isDecoImage)
                {
                    if(layer.linkedLayers == undefined || layer.linkedLayers.length == 0)
                    {
                        errorString.push("要裁剪的花纹图, 没有链接任何图层。");
                    }
                }
            }
            else
            {
                errorString.push("不支持的图层格式.");
            }
        
            if(nameSet[layerName] != undefined && !IsDetailImage(layerName))
            {
                errorString.push("图层重名,请改名或使用自动重命名功能.");
            }
            else
            {
                nameSet[layerName] = 1;
            }
        }
        else if(layerType == LAYERTYPE_LAYERSET)
        {
            var error = CheckLayerSet(layer);
            if(error != undefined)
            {
                errorString.push(error);
            }
        }
        
        if(errorString.length > 0)
        {
            var errorLayer = new Object();
            errorLayer.name = layerName;
            errorLayer.fullName = layer.name;
            errorLayer.errors = errorString;
            errorLayers.push(errorLayer);
        }
    }
}

function CenterLayer()
{
    var document = app.activeDocument;
    var docWidth = document.width.value;
    var docHeight = document.height.value;
    var activeLayer = document.activeLayer;
    if(activeLayer != undefined)
    {
        var left = activeLayer.bounds[0].value;
        var top = activeLayer.bounds[1].value;
        var right = activeLayer.bounds[2].value;
        var bottom = activeLayer.bounds[3].value;        
        var layerCenterX = left + (right - left) * 0.5;
        var layerCenterY = top + (bottom - top) * 0.5;
        var docCenterX = docWidth * 0.5;
        var docCenterY = docHeight * 0.5;
        
        var offset = new Array();
        offset.x = UnitValue(Math.round(docCenterX - layerCenterX), "px");
        offset.y = UnitValue(Math.round(docCenterY - layerCenterY), "px");
    
        for(var i=0; i<app.activeDocument.layers.length; i++)
        {
            var layer = app.activeDocument.layers[i];
            if(!layer.allLocked && layer.typename == "LayerSet" && layer.name != "Resources")
            {
                layer.translate(offset.x, offset.y)
            }
        }
    }
    else
    {
        alert("没有选中参考图层!");
    }
}

function FillLayerCommand(obj)
{    
    var layerCount = obj.layers.length
    var nextWithDetail = false
    var nextWithTill = false
    var detialInfo = ""
    var detailOrgBound = {};
    var opct = 100
    
    for(var i=0; i<layerCount; i++)
    {
        var layer = obj.layers[i];
        app.activeDocument.activeLayer = layer;
        
        var layerType = 0;
        if(layer.typename == "ArtLayer")
        {
            layerType = LAYERTYPE_ARTLAYER;
        }
        else if(layer.typename == "LayerSet")
        {
            layerType = LAYERTYPE_LAYERSET;
        }
        
        if(IsNoExportLayer(layer)) //No Export
        {
            var name = getLayerName(layer);
            layer.name = name + "@NE";
            continue;
        }
        else
        {
            if(layer.name.indexOf("@NE") != -1)
            {
                layer.name = layer.name.replace("@NE", "");
            }
        }
        
        if(layerType == LAYERTYPE_ARTLAYER)
        {
            //$.writeln (layer.name + layer.kind);
            if(layer.kind == LayerKind.TEXT)
            {     
                FillLabelCommand(layer);
            }
            else
            {
                // 处理@detail的情况

                if(layer.name.indexOf("@detailV") != -1)
                {
                    // 获取图层透明度
                    opct = layer.opacity;
                    detialInfo = getImageName(layer);

                    nextWithDetail = true;

                    selectionFromLayerSelf();

                    var selection = app.activeDocument.selection;
                    if(selection != undefined)
                    {
                        detailOrgBound[0] = selection.bounds[0]
                        detailOrgBound[1] = selection.bounds[3]
                        detailOrgBound[2] = selection.bounds[2]
                        detailOrgBound[3] = selection.bounds[1]     

                        selection.deselect();
                    }
                }
                else if(layer.name.indexOf("@detailT") != -1)
                {
                     // 获取图层透明度
                    opct = layer.opacity;
                    detialInfo = getImageName(layer);
                    nextWithTill = true;

                    selectionFromLayerSelf();

                    var selection = app.activeDocument.selection;
                    if(selection != undefined)
                    {
                        detailOrgBound[0] = selection.bounds[0]
                        detailOrgBound[1] = selection.bounds[1]
                        detailOrgBound[2] = selection.bounds[2]
                        detailOrgBound[3] = selection.bounds[3]     

                        selection.deselect();
                    }
                }
                else if(layer.name.indexOf("@detail") != -1)
                {
                    // 获取图层透明度
                    opct = layer.opacity;
                    detialInfo = getImageName(layer);

                    nextWithDetail = true;

                    selectionFromLayerSelf();

                    var selection = app.activeDocument.selection;
                    if(selection != undefined)
                    {
                        detailOrgBound[0] = selection.bounds[0]
                        detailOrgBound[1] = selection.bounds[1]
                        detailOrgBound[2] = selection.bounds[2]
                        detailOrgBound[3] = selection.bounds[3]     

                        selection.deselect();
                    }
                }
                else
                {
                    if(hasLayerMask())
                    {
                        FillSliceArea(layer);
                    }
                
                    var hasScreen = FillLayerEffectCommand(layer);
                    
                    var isDecoImage = false;
                    if(IsDecoImage(getImageName(layer)))
                    {
                        isDecoImage = FillLayerClipCommand(layer);
                    }
                    
                    if(nextWithDetail)
                    {
                        var leftOfMask        = detailOrgBound[0]
                        var topOfMask         = detailOrgBound[1]
                        var rightOfMask       = detailOrgBound[2]
                        var bottomOfMask      = detailOrgBound[3]

                        var leftOfMine        = layer.bounds[0]
                        var topOfMine         = layer.bounds[1]
                        var rightOfMine       = layer.bounds[2]
                        var bottomOfMine      = layer.bounds[3]

                        var width = rightOfMask - leftOfMask;
                        var height = bottomOfMask - topOfMask;

                        var xstep1 = leftOfMine - leftOfMask;
                        var xstep2 = rightOfMine - leftOfMask;

                        var ystep1 = topOfMine - topOfMask;
                        var ystep2 = bottomOfMine - topOfMask;

                        var lt_x = xstep1 / width;
                        var lt_y = 1.0 - ystep1 / height;

                        var rb_x = xstep2 / width;
                        var rb_y = 1.0 - ystep2 / height;

                        layer.name = insertParamToString (layer.name, "@deut=", 1);
                        layer.name = insertParamToString (layer.name, "@mask=", "" 
                        + lt_x.toFixed(2) + "," + rb_y.toFixed(2) + ","
                        + lt_x.toFixed(2) + "," + lt_y.toFixed(2) + ","
                        + rb_x.toFixed(2) + "," + lt_y.toFixed(2) + ","
                        + rb_x.toFixed(2) + "," + rb_y.toFixed(2) + ",");

                        layer.name = insertParamToString (layer.name, "@maskopt=", opct.toFixed(2));
                        layer.name = insertParamToString (layer.name, "@maskname=", detialInfo);  
                    }
                    else if(nextWithTill)
                    {
                        var leftOfMask        = detailOrgBound[0]
                        var topOfMask         = detailOrgBound[1]
                        var rightOfMask       = detailOrgBound[2]
                        var bottomOfMask      = detailOrgBound[3]

                        var leftOfMine        = layer.bounds[0]
                        var topOfMine         = layer.bounds[1]
                        var rightOfMine       = layer.bounds[2]
                        var bottomOfMine      = layer.bounds[3]

                        var width = rightOfMask - leftOfMask;
                        var height = bottomOfMask - topOfMask;

                        var xstep1 = leftOfMine - leftOfMask;
                        var xstep2 = rightOfMine - leftOfMask;

                        var ystep1 = topOfMine - topOfMask;
                        var ystep2 = bottomOfMine - topOfMask;

                        var lt_x = xstep1 / width;
                        var lt_y = 1.0 - ystep1 / height;

                        var rb_x = xstep2 / width;
                        var rb_y = 1.0 - ystep2 / height;

                        layer.name = insertParamToString (layer.name, "@deut=", 2);
                        layer.name = insertParamToString (layer.name, "@mask=", "" 
                        + lt_x.toFixed(2) + "," + rb_y.toFixed(2) + ","
                        + lt_x.toFixed(2) + "," + lt_y.toFixed(2) + ","
                        + rb_x.toFixed(2) + "," + lt_y.toFixed(2) + ","
                        + rb_x.toFixed(2) + "," + rb_y.toFixed(2) + ",");

                        layer.name = insertParamToString (layer.name, "@maskopt=", opct.toFixed(2));
                        layer.name = insertParamToString (layer.name, "@maskname=", detialInfo);  
                    }
                    else if(hasScreen)
                    {
                        layer.name = insertParamToString (layer.name, "@deut=", 4);
                    }
                    else
                    {
                        if(!isDecoImage)
                        {
                            layer.name = removeParamFromString(layer.name, "@deut=");
                            layer.name = removeParamFromString(layer.name, "@mask=");
                            layer.name = removeParamFromString(layer.name, "@maskopt=");
                            layer.name = removeParamFromString(layer.name, "@maskname=");
                        }
                    }
                     
                    nextWithDetail = false;
                    nextWithTill = false;
                }
            }
        
            if(Math.abs(layer.fillOpacity - 100.0) > 1)
            {
                layer.name = insertParamToString(layer.name, "@fopa=", (layer.fillOpacity / 100.0).toFixed(2));
            }
        }
        else if(layerType == LAYERTYPE_LAYERSET)
        {
            FillLayerCommand(layer);
        }
    }
}

function CollectDetailImage(obj, detailImages)
{
    var layerCount = obj.layers.length
    for(var i=0; i<layerCount; i++)
    {
        var layer = obj.layers[i];
        
        if(layer.typename == "ArtLayer")
        {
            if(IsDetailImage(getLayerName(layer)))
            {
                var nextLayer = undefined;
                for(var j=i+1; j<layerCount; j++)
                {
                    if(!IsDetailImage(getLayerName(obj.layers[j])))
                    {
                        nextLayer = obj.layers[j];
                        break;
                    }
                }
                
                var di = new Object();
                di.layer = layer;
                di.nextLayer = nextLayer;
                di.parent = layer.parent;
                detailImages.push(di);
                
            }
        }
    
        if(layer.typename == "LayerSet" && layer.name != "Resources")
        {
            CollectDetailImage(layer, detailImages);
        }
    }
}

function FillLayerSetCommand(obj)
{
    
    
    var layerCount = obj.layers.length
    for(var i=0; i<layerCount; i++)
    {
        var layer = obj.layers[i];
        app.activeDocument.activeLayer = layer;
        
        if(IsNoExportLayer(layer)) //No Export
        {
            continue;
        }
    
        if(layer.typename == "LayerSet" && layer.name != "Resources")
        {
            FillLayoutCommand(layer);
            
            FillLayerSetCommand(layer);
        }
    }
}

function FillLayerEffectCommand(layer)
{
    var name = layer.name;
    var sb = new StringBuffer();
    var hasGradientBlend = false;
    var hasScreen = false;
    var gradientParam = getGradientFill();    
    if(gradientParam)
    {
        var gradientColors = gradientParam.colorArray;
        var colorCount = Math.min(gradientColors.length, 5);
        if(colorCount > 0)
        {
            for(var i=colorCount-1; i>=0; i--)
            {
                var gColor = ((gradientColors[i].r << 16)
                            + (gradientColors[i].g << 8)
                            + (gradientColors[i].b)) >>> 0;
                    sb.append(FormatNum(gColor.toString(16), 6) + ',');
            }
        }
        
        name = insertParamToString (name, "@gcolor=", sb.toString());
        name = insertParamToString (name, "@gangle=", gradientParam.Angle < 0 ? 360 +gradientParam.Angle : gradientParam.Angle );
        
        hasScreen = gradientParam.hasScreen;
        // if(gradientParam.hasScreen)
        // {
        //     name = insertParamToString (name, "@gblendmode=", "add");
        // }
        // else
        // {
        //     name = insertParamToString (name, "@gblendmode=", "multi");
        // }
    }
    else
    {
        //sb.append("@color=FFFFFF");
    }

    layer.name = name;
    
    return hasScreen;
}

function FillLayerClipCommand(layer)
{
    if(layer.linkedLayers != undefined && layer.linkedLayers.length > 0)
    {
        var maskLayer = layer.linkedLayers[layer.linkedLayers.length - 1];
        var maskLayerName = getImageName(maskLayer);
        if(!IsDecoImage(maskLayerName) && !IsDetailImage(maskLayerName))
        {
            selectLayer(maskLayer.name);
            selectionFromLayerSelf();
            makePathFromSelection();
            var pathPoints = getPathPoints("工作路径");
            pathPoints = CollapsePath(pathPoints, 4);
            
            if(pathPoints != undefined && pathPoints.length >= 4)
            {
                var axis_X = 0;
                var axis_y = 0;
                pathPoints.sort(function(a, b){ return b.x - a.x; })
                axis_x = pathPoints[1].x + (pathPoints[2].x - pathPoints[1].x) / 2;
                pathPoints.sort(function(a, b){ return b.y - a.y; })
                axis_y = pathPoints[1].y + (pathPoints[2].y - pathPoints[1].y) / 2;
                
                var lt_point = pathPoints[0];
                var rt_point = pathPoints[0];
                var rb_point = pathPoints[0];
                var lb_point = pathPoints[0];
                for(var i=0; i<pathPoints.length; ++i)
                {
                    var px = pathPoints[i].x;
                    var py = pathPoints[i].y;
                    if(px < axis_x && py < axis_y)
                    {
                        lt_point = pathPoints[i];
                    }
                    else if(px > axis_x && py < axis_y)
                    {
                        rt_point = pathPoints[i];
                    }
                    else if(px > axis_x && py > axis_y)
                    {
                        rb_point = pathPoints[i];
                    }
                    else if(px < axis_x && py > axis_y)
                    {
                        lb_point = pathPoints[i];
                    }
                }
                pathPoints[0] = lt_point;
                pathPoints[1] = rt_point;
                pathPoints[2] = rb_point;
                pathPoints[3] = lb_point;
                
                var decoBounds = layer.bounds;
                var width = decoBounds[2].value - decoBounds[0].value;
                var height = decoBounds[3].value - decoBounds[1].value;

                var uv = new Array();
                uv[0] = new Object();
                uv[1] = new Object();
                uv[2] = new Object();
                uv[3] = new Object();
                
                //left top
                if(Math.abs(pathPoints[3].x - pathPoints[0].x) < 0.001)
                {
                    uv[0].u = ClampNum((pathPoints[0].x - decoBounds[0].value) / width, 0, 1);
                }
                else
                {
                    var xx = pathPoints[0].x + ((decoBounds[1].value - pathPoints[0].y) / (pathPoints[3].y - pathPoints[0].y)) * (pathPoints[3].x - pathPoints[0].x);
                    uv[0].u = ClampNum((xx - decoBounds[0].value) / width, 0, 1);
                }
            
                if(Math.abs(pathPoints[1].y - pathPoints[0].y) < 0.001)
                {
                    uv[0].v = ClampNum((pathPoints[0].y - decoBounds[1].value) / height, 0, 1);
                }
                else
                {
                    var yy = pathPoints[0].y + ((decoBounds[0].value - pathPoints[0].x) / (pathPoints[1].x - pathPoints[0].x)) * (pathPoints[1].y - pathPoints[0].y);
                    uv[0].v = ClampNum((yy - decoBounds[1].value) / height, 0, 1);
                }
            
                // right top
                if(Math.abs(pathPoints[2].x - pathPoints[1].x) < 0.001)
                {
                    uv[1].u = ClampNum((pathPoints[1].x - decoBounds[0].value) / width, 0, 1);
                }
                else
                {
                    var xx = pathPoints[1].x + ((decoBounds[1].value - pathPoints[1].y) / (pathPoints[2].y - pathPoints[1].y)) * (pathPoints[2].x - pathPoints[1].x);
                    uv[1].u = ClampNum((xx - decoBounds[0].value) / width, 0, 1);
                }
            
                if(Math.abs(pathPoints[1].y - pathPoints[0].y) < 0.001)
                {
                    uv[1].v = ClampNum((pathPoints[0].y - decoBounds[1].value) / height, 0, 1);
                }
                else
                {
                    var yy = pathPoints[0].y + ((decoBounds[2].value - pathPoints[0].x) / (pathPoints[1].x - pathPoints[0].x)) * (pathPoints[1].y - pathPoints[0].y);
                    uv[1].v = ClampNum((yy - decoBounds[1].value) / height, 0, 1);
                }
            
                // right bottom
                if(Math.abs(pathPoints[2].x - pathPoints[1].x) < 0.001)
                {
                    uv[2].u = ClampNum((pathPoints[1].x - decoBounds[0].value) / width, 0, 1);
                }
                else
                {
                    var xx = pathPoints[1].x + ((decoBounds[3].value - pathPoints[1].y) / (pathPoints[2].y - pathPoints[1].y)) * (pathPoints[2].x - pathPoints[1].x);
                    uv[2].u = ClampNum((xx - decoBounds[0].value) / width, 0, 1);
                }
            
                if(Math.abs(pathPoints[2].y - pathPoints[3].y) < 0.001)
                {
                    uv[2].v = ClampNum((pathPoints[3].y - decoBounds[1].value) / height, 0, 1);
                }
                else
                {
                    var yy = pathPoints[3].y + ((decoBounds[2].value - pathPoints[3].x) / (pathPoints[2].x - pathPoints[3].x)) * (pathPoints[2].y - pathPoints[3].y);
                    uv[2].v = ClampNum((yy - decoBounds[1].value) / height, 0, 1);
                }
            
                // left bottom
                if(Math.abs(pathPoints[3].x - pathPoints[0].x) < 0.001)
                {
                    uv[3].u = ClampNum((pathPoints[0].x - decoBounds[0].value) / width, 0, 1);
                }
                else
                {
                    var xx = pathPoints[0].x + ((decoBounds[3].value - pathPoints[0].y) / (pathPoints[3].y - pathPoints[0].y)) * (pathPoints[3].x - pathPoints[0].x);
                    uv[3].u = ClampNum((xx - decoBounds[0].value) / width, 0, 1);
                }
            
                if(Math.abs(pathPoints[3].y - pathPoints[2].y) < 0.001)
                {
                    uv[3].v = ClampNum((pathPoints[3].y - decoBounds[1].value) / height, 0, 1);
                }
                else
                {
                    var yy = pathPoints[3].y + ((decoBounds[0].value - pathPoints[3].x) / (pathPoints[2].x - pathPoints[3].x)) * (pathPoints[2].y - pathPoints[3].y);
                    uv[3].v = ClampNum((yy - decoBounds[1].value) / height, 0, 1);
                }
                
                layer.name = insertParamToString(layer.name, "@deut=", 3);
                layer.name = insertParamToString(layer.name, "@mask=", "" 
                                + uv[3].u.toFixed(3) + "," + (1.0 - uv[3].v).toFixed(3) + ","
                                + uv[0].u.toFixed(3) + "," + (1.0 - uv[0].v).toFixed(3) + ","
                                + uv[1].u.toFixed(3) + "," + (1.0 - uv[1].v).toFixed(3) + ","
                                + uv[2].u.toFixed(3) + "," + (1.0 - uv[2].v).toFixed(3) + ",");
                                
                return true;
            }
        }
    }

    return false;
}

function FillLabelCommand(layer)
{
    var name = getLayerName(layer);
    var sb = new StringBuffer();
    
    var textItem = layer.textItem;
    
    var textAlign = "left";
    var pivot = 0;
    try
    {
        var justification = textItem.justification
        if(justification == Justification.RIGHT ||
            justification == Justification.RIGHTJUSTIFIED)
        {
            textAlign = "right";
            pivot = 1.0
        }
        else if(justification == Justification.LEFT ||
                 justification == Justification.LEFTJUSTIFIED)
        {
            textAlign = "left";
            pivot = 0
        }
        else
        {
            textAlign = "center";
            pivot = 0.5
        }
    }
    catch(e){}
    
    if(textAlign != "center")
    {
        sb.append("@textalign=" + textAlign);
    }    
    
    
    var color = 0xffffff;
    try
    {
        color = textItem.color.rgb.hexValue;
    }
    catch(e){}

    var rotate = getTextLayerRotation();
    var scale = getTextLayerScale();
    if(Math.abs(rotate) > 1)
    {
        var xxscale = Math.sin((rotate + 90) * (Math.PI / 180));
        scale = scale / Math.abs(xxscale);
    }
    
    var size = Math.round(textItem.size.value * scale);
    
    var posx = 0;
    var posy = 0;
    try
    {   
        var left = layer.boundsNoEffects[0].value;
        var top = layer.boundsNoEffects[1].value;
        var right = layer.boundsNoEffects[2].value;
        var bottom = layer.boundsNoEffects[3].value;
        if(Math.abs(rotate) < 1)
        {
            
            posx = (right - left) * pivot + left;
            //posx = textItem.position[0].value;
        }
        else
        {
            posx = (right - left) * 0.5 + left;
        }
        posy = (bottom - top) * 0.5 + top;
    }
    catch(e){}
    
    sb.append("@pos=" + posx.toFixed(1) + ',' + posy.toFixed(1));
    sb.append("@size=" + size);
    
    if(Math.abs(rotate) > 1)
    {
        sb.append("@rotate=" + rotate);
    }
    
    var fontStyle = "normal";
    var isBold = false;
    var isItalic = false;
    var textCase = false;
    try
    {
        isBold = textItem.fauxBold;
    }
    catch(e){}

    try
    {
        isItalic = textItem.fauxItalic;
    }
    catch(e){}

    if(isBold && isItalic)
    {
        fontStyle = "boldanditalic";
    }
    else if(isBold)
    {
        fontStyle = "bold";
    }
    else if(isItalic)
    {
        fontStyle = "italic";
    }

    if(fontStyle != "normal")
    {
        sb.append("@fontstyle=" + fontStyle);
    }

    try
    {
        textCase = textItem.capitalization;
        if(textCase == TextCase.ALLCAPS)
        {
            sb.append("@caps")
        }
    }
    catch(e){}
    
    try
    {
        var characterSpacing = textItem.tracking ? textItem.tracking * 0.001 * size : 0;
        sb.append("@spacing=" + characterSpacing);
        
    }
    catch(e){}
    
    try
    {
        var leading = textItem.leading ? textItem.leading.value - textItem.size.value : 0;
        sb.append("@lead=" + leading)
    }
    catch(e){}
    
    try
    {
        var font = textItem.font;
        if(font.indexOf("MicrosoftYaHei-Bold") != -1 ||
            font.indexOf("FZLTDHJW--GB1-0") != -1 ||
            font.indexOf("FZLTDHK--GBK1-0") != -1)
        {
            sb.append("@font=FZZH");
        }
    }
    catch(e){}

    var dropShadowEffect = getDropShadowEffect();
    if(dropShadowEffect)
    {
        sb.append("@effectType=shadow");
        
        var shadowColor = ((dropShadowEffect.color[0] << 24)
                   + (dropShadowEffect.color[1] << 16) 
                   + (dropShadowEffect.color[2] << 8) 
                   + (dropShadowEffect.color[3])) >>> 0;
        sb.append("@effectcolor=" + FormatNum(shadowColor.toString(16), 8));
        sb.append("@effectsize=" + dropShadowEffect.x + ',' + dropShadowEffect.y);
    }

    var frameFxEffect = getFrameFxEffect();
    if(frameFxEffect)
    {
        sb.append("@effectType=outline");
        var frameFxColor = ((frameFxEffect.color[0] << 24)
                   + (frameFxEffect.color[1] << 16) 
                   + (frameFxEffect.color[2] << 8) 
                   + (frameFxEffect.color[3])) >>> 0;
        sb.append("@effectcolor=" + FormatNum(frameFxColor.toString(16), 8));
        sb.append("@effectsize=" + frameFxEffect.size + ',' + frameFxEffect.size);
    }

    var outerGlowEffect = getOuterGlowEffect();
    if(outerGlowEffect)
    {
        sb.append("@outerglow");
        var outerglowColor = ((outerGlowEffect.color[0] << 24)
                   + (outerGlowEffect.color[1] << 16) 
                   + (outerGlowEffect.color[2] << 8) 
                   + (outerGlowEffect.color[3])) >>> 0;
        sb.append("@outercolor=" + FormatNum(outerglowColor.toString(16), 8));
        //sb.append("@outersize=" + outerGlowEffect.size)
    }

    var hasGradientBlend = false;
    var gradientParam = getGradientFill();
    if(gradientParam)
    {
        var gradientColors = gradientParam.colorArray;
        var colorCount = Math.min(gradientColors.length, 5);
        if(colorCount > 0)
        {
            sb.append("@gcolor=");
            if(gradientParam.isUp)
            {
                for(var i=colorCount-1; i>=0; i--)
                {
                    var gColor = ((gradientColors[i].r << 16)
                               + (gradientColors[i].g << 8)
                               + (gradientColors[i].b)) >>> 0;
                    sb.append(FormatNum(gColor.toString(16), 6) + ',');
                }
            }
            else
            {
                for(var i=0; i<colorCount; i++)
                {
                    var gColor = ((gradientColors[i].r << 16)
                               + (gradientColors[i].g << 8)
                               + (gradientColors[i].b)) >>> 0;
                    sb.append(FormatNum(gColor.toString(16), 6) + ',');
                }
            }
        }
        
        if(gradientParam.hasBlend)
        {
            sb.append("@color=" + color);
        }
        else
        {
            sb.append("@color=FFFFFF");
        }
    }
    else
    {
        sb.append("@color=" + color);
    }

    var newName = name + sb.toString();
        //+ "@color=" + color
        //+ "@size=" + size 
        //+ "@fontstyle=" + fontStyle
        //+ "@spacing=" + characterSpacing
        //+ "@textalign=" + textAlign;
    
    layer.name = newName;
}

function getSliceAreaParam(layer, isSelection)
{
    if(isSelection)
    {
        selectionFromLayerMask();
    }
    else
    {
        selectionFromLayerMask(layer.name);
    }
    
    var retParam = new Array();
    var selection = app.activeDocument.selection;
    if(selection != undefined)
    {
        selection.invert();
        
        retParam[0] = Math.max(selection.bounds[0].value - layer.bounds[0].value, 0);
        retParam[1] = Math.max(selection.bounds[1].value - layer.bounds[1].value, 0);
        retParam[2] = Math.max(layer.bounds[2].value - selection.bounds[2].value, 0);
        retParam[3] = Math.max(layer.bounds[3].value - selection.bounds[3].value, 0);
        
        selection.deselect();
    }

    return retParam;
}

function FillSliceArea(layer)
{
    var name = layer.name; 
    var sliceParam = getSliceAreaParam(layer, true);
    if(sliceParam != undefined && sliceParam.length > 0)
    {
        var valueStr = "@slice=" + sliceParam[0] + 'x' + sliceParam[1] + 'x' + sliceParam[2] + 'x' + sliceParam[3];
        if(name.indexOf("@slice=") != -1)
        {
            name = name.replace(/@slice=[^@]*([@.*]{0,1})/, valueStr + "$1");
        }
        else
        {
            name = name + valueStr;
        }
    
        layer.name = name;
    }
    else
    {
        layer.name = removeParamFromString(name, "@slice=")
    }
}

function CalculateArrangeSortV(a, b)
{
    if(a.y == b.y)
    {
        return a.x - b.x;
    }
    else
    {
        return a.y - b.y;
    }
}

function CalculateArrangeSortH(a, b)
{
    if(a.x == b.x)
    {
        return a.y - b.y;
    }
    else
    {
        return a.x - b.x;
    }
}

function CalculateArrangeParam(layerset)
{
    var childCount = layerset.layers.length;
    var cellWidth = 0;
    var cellHeight = 0;
    var hCount = 0;
    var vCount = 0;
    var hasUnusualStep = false;
    var isHorizontal = (layerset.name.indexOf("@dirc=h") != -1);
    var itemWidth = 0;
    var itemHeight = 0;
    
    if(childCount > 0)
    {
        var layerArray = new Array()
        for(var i=0; i<childCount; i++)
        {
            var bounds = layerset.layers[i].bounds;
            var layerObj = new Object();
            layerObj.width = bounds[2].value - bounds[0].value
            layerObj.height = bounds[3].value - bounds[1].value
            layerObj.x = bounds[0].value + layerObj.width / 2;
            layerObj.y = bounds[1].value + layerObj.height / 2;
            layerArray[i] = layerObj;
        }
        
        itemWidth = layerArray[0].width;
        itemHeight = layerArray[0].height
        
        if(isHorizontal)
        {
            layerArray.sort(CalculateArrangeSortH);
        }
        else
        {
            layerArray.sort(CalculateArrangeSortV);
        }
    
        var posXMin = layerArray[0].x;
        var posXMax = posXMin;
        var posYMin = layerArray[0].y;
        var posYMax = posYMin;
        hCount = 1;
        vCount = 1;
        for(var i=1; i<childCount; i++)
        {
            var x = layerArray[i].x;
            var y = layerArray[i].y;
            
            if(x < posXMin)
            {
                var cw = posXMin - x;
                if(cw > cellWidth)
                {
                    cellWidth = cw;
                }
                posXMin = x;
                ++hCount;
            }
            else if(x > posXMax)
            {
                var cw = x - posXMax;
                if(cw > cellWidth)
                {
                    cellWidth = cw;
                }
                posXMax = x;
                ++hCount;
            }
            
            if(y < posYMin)
            {
                var ch = posYMin - y;
                if(ch > cellHeight)
                {
                    cellHeight = ch;
                }
                posYMin = y;
                ++vCount;
            }
            else if(y > posYMax)
            {
                var ch = y - posYMax;
                if(ch > cellHeight)
                {
                    cellHeight = ch;
                }
                posYMax = y;
                ++vCount;
            }
        }
    
        if(layerArray.length > 0)
        {
            if(cellWidth == 0)
            {
                cellWidth = layerArray[0].width
            }
            if(cellHeight == 0)
            {
                cellHeight = layerArray[0].height
            }
        }
    }

    if(hCount * vCount != childCount)
    {
        hasUnusualStep = true;
    }

    var result = new Object();
    result.CellWidth = Math.round(cellWidth);
    result.CellHeight = Math.round(cellHeight);
    result.ItemWidth = itemWidth;
    result.ItemHeight = itemHeight;
    result.Rows = vCount;
    result.Columns = hCount;
    result.HasUnusualStep = hasUnusualStep;
    return result;
}

function FillArrangeCommand(layer, name)
{   
    var arrangeParam = CalculateArrangeParam (layer);
    if(arrangeParam.CellWidth != 0)
    {
        name = insertParamToString (name, "@gdcw=", arrangeParam.CellWidth);
    }
    if(arrangeParam.CellHeight != 0)
    {
        name = insertParamToString (name, "@gdch=", arrangeParam.CellHeight);
    }
    if(arrangeParam.Rows != 0)
    {
        name = insertParamToString (name, "@rows=", arrangeParam.Rows);
    }
    if(arrangeParam.Columns != 0)
    {
        name = insertParamToString (name, "@coln=", arrangeParam.Columns);
    }
    if(arrangeParam.ItemWidth != 0)
    {
        name = insertParamToString (name, "@itmw=", arrangeParam.ItemWidth);
    }
    if(arrangeParam.ItemHeight != 0)
    {
        name = insertParamToString (name, "@itmh=", arrangeParam.ItemHeight);
    }

    if(arrangeParam.HasUnusualStep)
    {
        alert("Wraning, grid has not alignment.")
    }

    return name;
}

function CalculateClipParam(layerset)
{
    var childCount = layerset.layers.length;
    if(childCount > 0)
    {
        for(var i=1; i<childCount; i++)
        {
            var layer = layerset.layers[i];
            if(layer.typename == "ArtLayer" &&
               layer.kind == LayerKind.SOLIDFILL)
            {
                return layer.bounds;
            }
        }
    }

    return undefined;
}

function FillLayoutCommand(layer)
{
    var name = layer.name;
    var bounds = layer.bounds;
    var width = bounds[2].value - bounds[0].value;
    var height = bounds[3].value - bounds[1].value;
    var x = bounds[0].value + width / 2.0;
    var y = bounds[1].value + height / 2.0; 
    
    if(name.indexOf("@panel") != -1)
    {
        var clipBound = CalculateClipParam(layer);
        if(clipBound != undefined)
        {
            width = clipBound[2].value - clipBound[0].value;
            height = clipBound[3].value - clipBound[1].value;
            x = clipBound[0].value + width / 2.0;
            y = clipBound[1].value + height / 2.0;
        }
    
        name = insertParamToString(name, "@clip=", x + ',' + y + ',' + width + ',' + height);   
        layer.name = name;
    }
    else if(name.indexOf("@grid") != -1)
    {
        name = insertParamToString(name, "@area=", bounds[0].value + ',' + bounds[1].value + ',' + bounds[2].value + ',' + bounds[3].value);
        name = FillArrangeCommand(layer, name)

        layer.name = name;
    }
    else if(name.indexOf("@table") != -1)
    {
        name = insertParamToString(name, "@area=", bounds[0].value + ',' + bounds[1].value + ',' + bounds[2].value + ',' + bounds[3].value);
        name = FillArrangeCommand(layer, name)

        layer.name = name;
    }
    else if(name.indexOf("@wrapcontent") != -1)
    {
        name = insertParamToString(name, "@area=", bounds[0].value + ',' + bounds[1].value + ',' + bounds[2].value + ',' + bounds[3].value);
        name = FillArrangeCommand(layer, name)

        layer.name = name;
    }
    else
    {
        if(x != 0 || y != 0)
        {
            name = insertParamToString(name, "@pos=", x.toFixed(1)+',' + y.toFixed(1));    
        }
        else
        {
            name = removeParamFromString (name, "@pos=");
        }
    
        if(name.indexOf("@arrg") != -1)
        {
            name = insertParamToString(name, "@area=", bounds[0].value + ',' + bounds[1].value + ',' + bounds[2].value + ',' + bounds[3].value);
        }
        
        layer.name = name;
    }
}

function CopyToResourcesRecursive(obj, resArray, copyArray, isCover)
{
    var layerCount = obj.layers.length
    for(var i=0; i<layerCount; i++)
    {
        var layer = obj.layers[i];
        app.activeDocument.activeLayer = layer;

        if(layer.typename == "ArtLayer")
        {
            if(layer.kind == LayerKind.NORMAL)
            {
                var imageName = getImageName(layer);
                if(resArray[imageName] != undefined)
                {
                    if(isCover)
                    {
                        var newImageName = imageName + " copy";
                        DuplicateLayer(newImageName);
                        copyArray[imageName] = 1;
                        copyArray[newImageName] = 2;
                    }
                }
                else
                {
                    DuplicateLayer(imageName);
                    copyArray[imageName] = 0;
                }
            }
        }
        else if(layer.typename == "LayerSet")
        {
            CopyToResourcesRecursive(layer, resArray, copyArray, isCover);
        }
    }
}

function MoveResourcesRecursive(obj, resourcesLayer, copyArray, opArray)
{ 
    for(var ci=0; ci<obj.layers.length; ci++)
    {
        var layer = obj.layers[ci];
        //app.activeDocument.activeLayer = layer;
        
        if(layer.typename == "ArtLayer")
        {
            var imageName = getImageName(layer);
            var copyType = copyArray[imageName];
            if(copyType != undefined)
            {
                var op = new Object();
                op.layer = layer;
                op.imageName = imageName;
                op.type = copyType;
                opArray.push(op);
            }
        }
        else if(layer.typename == "LayerSet")
        {
            MoveResourcesRecursive(layer, resourcesLayer, copyArray, opArray);
        }
    }
}

function CopyToResources(isCover)
{
    if(app.activeDocument.name == "Resources.psd")
        return;
    
    var resources = new Array();
    var collectResRet = CollectResources(resources);
    if(collectResRet != true)
    {
        WarningDialog(collectResRet);
        return;
    }

    var copyArray = undefined;
    for(var i=0; i<app.activeDocument.layers.length; i++)
    {
        var layer = app.activeDocument.layers[i];
        if(layer.name == "Resources")
        {
            copyArray = new Array();
            CopyToResourcesRecursive(layer, resources, copyArray, isCover);
            break;
        }
    }

    if(copyArray == undefined)
    {
        alert("没有找到Resources组!");
        return;
    }

    var currentDocument = app.activeDocument
    var resDoc = undefined;
    try
    {
        resDoc = app.documents.getByName("Resources.psd");
    }
    catch(e){}

    if(resDoc == undefined)
    {
        return "Resources.psd 不存在!";
    }

    app.activeDocument = resDoc;
    {
        var resourcesLayer = undefined;
        for(var ci=0; ci<app.activeDocument.layers.length; ci++)
        {
            if(app.activeDocument.layers[ci].name == "Resources")
            {
                resourcesLayer = app.activeDocument.layers[ci];
                break;
            }
        }
        
        if(resourcesLayer != undefined)
        {
            var opArray = new Array();
            MoveResourcesRecursive(app.activeDocument, resourcesLayer, copyArray, opArray);
        
            for(var opIndex=0; opIndex<opArray.length; opIndex++)
            {
                var layer = opArray[opIndex].layer;
                var imageName = opArray[opIndex].imageName;
                var opType = opArray[opIndex].type;
                if(layer != undefined)
                {
                    if(opType == 0)
                    {
                        layer.move(resourcesLayer, ElementPlacement.INSIDE);
                    }
                    else if(opType == 1)
                    {
                        layer.remove();
                    }
                    else if(opType == 2)
                    {
                        var subIndex = imageName.lastIndexOf(" copy");
                        if(subIndex > 0)
                        {
                            var name = imageName.substring(0, subIndex);
                            layer.name = name;
                        }
                        layer.move(resourcesLayer, ElementPlacement.INSIDE);
                    }
                }
            }
        }
        else
        {
            alert("没有找到Resources组!");
        }
    }
    app.activeDocument = currentDocument;
    
}

function CheckLayerName(obj, nameSet, count)
{
    var layerNum = obj.layers.length;
    for(var i=0; i<layerNum; ++i)
    {
        var layer = obj.layers[i];
        if(layer.allLocked || layer.name == "Resources")  //No Export
        {
            continue;
        }
    
        if(layer.typename == "LayerSet")
        {
            count = CheckLayerName(layer, nameSet, count);
        }
        count = count + 1;
        
        var name = getLayerName(layer);
        if(nameSet[name] != undefined && !IsDetailImage(name))
        {
            var newName = name;
            var sIndex = name.indexOf('#');
            if(layer.typename == "ArtLayer" && layer.kind == LayerKind.NORMAL && sIndex == -1)
            {
                newName = newName + '#' + count;
            }
            else
            {
                newName = newName + '_' + count;
            }
        
            layer.name = layer.name.replace(name, newName);
            nameSet[newName] = count;
            setLayerColorFlag(layer.name, COLORID_YELLOW);
        }
        else
        {
            nameSet[name] = count;
        }
    }

    return count;
}

function CleanLayerEffect(obj)
{
    var layerNum = obj.layers.length;
    for(var i=0; i<layerNum; ++i)
    {
        var layer = obj.layers[i];
        app.activeDocument.activeLayer = layer;
        
        if(layer.typename == "LayerSet")
        {
            CleanLayerEffect(layer)
        }
        else
        {
            var layerEffectArray = getAllLayerEffectKeys(false);
            if(layerEffectArray != undefined)
            {
                for(var j=0; j<layerEffectArray.length; ++j)
                {
                    deleteLayerEffect(layerEffectArray[j]);
                }
            }
        }
    }
    
    //LogActionObj(desc, 0);
}

function CleanLayerParam(obj)
{
    var layerNum = obj.layers.length;
    for(var i=0; i<layerNum; ++i)
    {
        var layer = obj.layers[i];
        if(layer.typename == "LayerSet")
        {
            CleanLayerParam(layer);
        }
        else
        {
            var whiteListParam = new Array();
            var layerName = layer.name;
            for(var j=1; j<CleanParamWhiteList.length; ++j)
            {
                var param = getParamFromString(layerName, CleanParamWhiteList[j]);
                if(param != undefined && param != "")
                {
                    whiteListParam.push(param);
                }
            }
        
            var name = getLayerName(layer) + whiteListParam.join('');
            layer.name = name;
        }
    }
}

function CleanNotUsedResources(resources)
{
    var currentDocument = app.activeDocument;   
    if(currentDocument.name != "Resources.psd")
    {
        alert("不是Resource.psd!");
        return;
    }
    
    touchUpLayerSelection();
    
    var resArray = new Array();
    CollectResourcesRecursive(currentDocument, resArray, RESOURCE_TYPE_SYSTEM);
    
    var folder = new Folder(currentDocument.path)
    if(!folder)
        return;
        
        
    var files = folder.getFiles("*.psd");
    if(!files)
        return;
    
    for(var i=0; i<files.length; ++i)
    {
        var file = files[i];
        if(file.name != "Resources.psd")
        {
            var doc = app.open(files[i]);
            if(doc != undefined)
            {
                CountResourcesUse(doc, doc.name, resArray);
                doc.close(SaveOptions.DONOTSAVECHANGES);
            }
        }
    }

    app.activeDocument = currentDocument;
    touchUpLayerSelection();
    for(var key in resArray)
    {
        var res = resArray[key];
        if(res.UseCount == undefined || res.UseCount == 0)
        {
            if(res.fullName != undefined)
            {
                setLayerColorFlag(res.fullName, COLORID_ORANGE);
            }
        }
    }
}

function CountResourcesUse(obj, docName, resources)
{
    var layerCount = obj.layers.length;
    for(var i=0; i<layerCount; ++i)
    {
        var layer = obj.layers[i];
        if(!IsNoExportLayer(layer))
        {
            if(layer.typename == "ArtLayer")
            {
                if(layer.kind == LayerKind.NORMAL)
                {
                    var imageName = getImageName(layer);
                    var resource = resources[imageName];
                    if(resource != undefined)
                    {
                        if(resource.UseCount != undefined)
                        {
                            resource.UseCount += 1;
                        }
                        else
                        {
                            resource.UseCount = 1;
                        }
                    
                        if(resource.UseRef == undefined)
                        {
                            resource.UseRef = new Array();
                            
                        }
                        resource.UseRef.push(docName);
                    }
                }
            }
            else if(layer.typename == "LayerSet")
            {
                if(layer.name != "Resources")
                {
                    CountResourcesUse(layer, docName, resources);
                }
            }
        }
    }
}

function initExportInfo(exportInfo)
{
    /*exportInfo.destination = new String("");
    exportInfo.fileNamePrefix = new String("untitled_");
    exportInfo.visibleOnly = false;
    exportInfo.fileType = psdIndex;
    exportInfo.icc = true;
    exportInfo.jpegQuality = 8;
    exportInfo.psdMaxComp = true;
    exportInfo.tiffCompression = TIFFEncoding.NONE;
    exportInfo.tiffJpegQuality = 8;
    exportInfo.pdfEncoding = PDFEncoding.JPEG;
    exportInfo.pdfJpegQuality = 8;
    exportInfo.targaDepth = TargaBitsPerPixels.TWENTYFOUR;
    exportInfo.bmpDepth = BMPDepthType.TWENTYFOUR;
    exportInfo.png24Transparency = true;
    exportInfo.png24Interlaced = false;
    exportInfo.png24Trim = true;
    exportInfo.png8Transparency = true;
    exportInfo.png8Interlaced = false;
    exportInfo.png8Trim = true;

    try {
        exportInfo.destination = Folder(app.activeDocument.fullName.parent).fsName; // destination folder
        var tmp = app.activeDocument.fullName.name;
        exportInfo.fileNamePrefix = decodeURI(tmp.substring(0, tmp.indexOf("."))); // filename body part
    } catch(someError) {
        exportInfo.destination = new String("");
        exportInfo.fileNamePrefix = app.activeDocument.name; // filename body part
    }*/
}