# DialogWindow
4x-ի AsDialog դասի համարժեքը 8x-ում հանդիսանում է DialogWindow դասը։

Կնկարագրենք օրինակ անցման համար։

Քայլ 1 `Դասի օբյեկտի ստեղծում`

`AS-4X`
```vba
Dim xDialog As AsDialog
Set xDialog = CreateDialog
	With xDialog
		.Caption = "Æñ³í³ëáõÃÛáõÝÝ»ñÇ ÷á÷áËáõÃÛáõÝÝ»ñ"
		.ECaption = "Access Changes"
  
```

`AS-8X`
```csharp
var dialog = new DialogWindow
{
    Caption = Resources.AccessChanges
};
```

Քայլ 2 `Անհրաժեշտ control-ների ավելացում պատուհանում`

`AS-4X`
```vba
 .AddControl "DateBegin", #Period, "Date", "R", Param("Wkdate") -1, #e_Period
 .AddControl "DateBegin", #Period, "Date", "R", Param("Wkdate") -1, #e_Period
 .AddDublCntrl "DateEnd", "DateBegin", "R", Param("Wkdate")
```

`AS-8X`
```csharp
var hLayout = dialog.AddHorizontalLayoutGroup();
var dateBegin = dialog.AddDateEditControl(hLayout, nameof(AccessLogParameters.DateBegin), Resources.DatePeriod, isRequired: true);
dateBegin.Value = Settings.WKDate.AddDays(-1);
var dateEnd = dialog.AddDateEditControl(hLayout, nameof(AccessLogParameters.DateEnd), "", isRequired: true);
dateEnd.Value = Settings.WKDate;
```
4x-ի Date տիպի control-ի համարժեքը 8x-ում DateEditControl-ն է։
8x-ում Date տիպի control-ների զույգ սահմանելու համար նախ անհրաժեշտ է ստեղծել Horizontal layout group, որի վրա էլ կավելացվեն 2 `DateEdit` control-ները:
dialog-ի AddDateEditControl մեթոդի միջոցով էլ DateEdit տիպի control-ը ավելացնում ենք ֆիլտրման պատուհանում։
Մեթոդը ունի հետևյալ պարամետրերը՝
| Անվանում | Տեսակ | Նկարագրություն |
| --- | --- | --- |
| name | string | control-ի ներքին անվանում |
| caption | string | control-ի վերնագիր |
| useLongDate | bool | control-ում արժեքը երևա երկար ֆորմատով թե ոչ  <br>(  <br>կարճ ֆորմատ - dd/mm/yy, օրինակ՝ 25/04/24  <br>երկար ֆորմատ - dd/mm/yyyy, օրինակ 25/04/2024  <br>) |
| isRequired | bool | control-ի արժեքի լրացումը պարտադիր է թե ոչ |
| storeValue | bool | DialogWindow-ն ունի control-ների արժեքները հիշելու հնարավորություն։ Այս հատկությամբ որոշվում է DialogWindow բացելուց հետո տվյալ control-ի արժեքը հիշվի թե ոչ ՞՞՞

4x-ի Boolean տիպի control-ի համարժեքը 8x-ում CheckEditExt-ն է։
`AS-4X`
```vba
.AddControl "bShowNonProcessedOnly", "òáõÛó ï³É ÙÇ³ÛÝ ãÙß³Ïí³ÍÝ»ñÁ", "BOOLEAN", , True
```

`AS-8X`
```csharp
var showAll = dialog.AddCheckEdit(nameof(AccessLogParameters.ShowAll), Resources.ShowNonProcessedOnly);
showAll.EditValue = true;
```
Մեթոդը ունի հետևյալ պարամետրերը՝
| Անվանում | Տեսակ | **Նկարագրություն** |
| --- | --- | --- |
| name | string | control-ի ներքին անվանում |
| caption | string | control-ի վերնագիր |
| storeValue | bool | DialogWindow-ն ունի control-ների արժեքները հիշելու հնարավորություն։ Այս հատկությամբ որոշվում է DialogWindow բացելուց հետո տվյալ control-ի արժեքը հիշվի թե ոչ ՞՞՞ |
| isThreeState | bool | control-ի նշիչը ունենա 2 թե 3 վիճակ |






