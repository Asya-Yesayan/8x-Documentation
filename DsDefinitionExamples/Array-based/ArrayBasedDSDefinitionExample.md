# Ինչպես նկարագրել array-based տվյալների աղբյուր

Ամբողջական կոդը դիտելու համար [տես](ArrayBasedDSDefinitionExampleCode.cs)։ 

- Անհրաժեշտ է հայտատարել դաս, որը ժառանգում է DataSource<R, P> դասը, որտեղ որպես R անհրաժեշտ է փոխանցել տվյալների աղբյուրի սյուները նկարագրող դաս, իսկ որպես P` տվյալների աղբյուրի պարամետրերը նկարագրող դաս։

```c#
    public class ProcMode : DataSource<ProcMode.DataRow, ProcMode.Param>
```

Եթե տվյալների աղբյուրը չի պարունակում պարամետրեր, ապա որպես P անհրաժեշտ է փոխանցել NoParam դասը։
```c#
public class ApiClientInfo : DataSource<ApiClientInfo.DataRow, NoParam>
```

Տվյալների աղբյուրի սյուները նկարագրող դասը պետք է իմպլեմենտացնի IExtendableRow ինտերֆեյսը։ Այդ դասում անհրաժեշտ է որպես հատկություններ ավելացնել տվյալների աղբյուրի սյուները։
Տվյալների աղբյուրի սյուները նկարագրող դասի օրինակ՝
```c#
        public class DataRow : IExtendableRow
        {
            public short CODE { get; set; }
            public string DESC { get; set; }
            public object Extend { get; set; }
        }
```

Տվյալների աղբյուրի պարամետրերը նկարագրող դասում նույնպես պետք է որպես հատկություններ պետք է ավելացնել պարամետրը։
Տվյալների աղբյուրի պարամետրերը նկարագրող դասի օրինակ՝
```c#
        public class Param
        {
            public short SourceType { get; set; }
        }
```

- Հետո անհրաժեշտ է ձևավորել տվյալների աղբյուրի կոնստրուկտորը, որը իր հերթին պիտի կանչի base DataSource<R, P> դասի կոնստրուկտորը: Կոնստուկտորում անհրաժեշտ է inject անել բոլոր այն service-ները, որոնք հետագայում հարկավոր են լինելու աշխատանքի համար։
Տվյալների աղբյուրի կոնստրուկտորում պարտադիր պետք է ունենալ IServiceProvider տիպի պարամետր, որն էլ պետք է փոխանցել base դասի կոնստրուկտորին։
```c#
       public ProcMode(IServiceProvider serviceProvider) : base(serviceProvider)
       {
              ․․․․․
       }
```
Կոնստրուկտորում ձևավորվում է տվյալների աղբյուրի սխեման, որը պարունակում է ամբողջական ինֆորմացիա տվյալների աղբյուրի սյուների ու պարամետրերի մասին։

base դասի Schema հատկության անհրաժեշտ է վերագրել Schema դասի նոր օբյեկտ։
Schema դասի կոնստրուկտորը ունի հետևյալ կառուցվածքը՝
```c#
        public Schema(string name, string armenianCaption, string englishCaption, Type rowType, Type paramType,
                      bool supportedExtendedFeatures = false) : base(name, armenianCaption, englishCaption, supportedExtendedFeatures)
```
Կոնստրուկտորի պարամետրի մասին ամբողջական ինֆորմացիան ներկայացված է ստորև՝

| Անվանում | Տեսակ | **Նկարագրություն** |
| --- | --- | --- |
| name | string | Սխեմայի ներքին անվանում|
| armenianCaption | string | Սխեմայի հայերեն անվանում(անվանումը անհրաժեշտ է փոխանցել Ansi կոդավորմամբ)|
| englishCaption | string | Սխեմայի անգլերեն անվանում|
| rowType | Type | Տվյալների աղբյուրի սյուները նկարագրող դասի տիպը|
| paramType | Type | Տվյալների աղբյուրի պարամետրերը նկարագրող դասի տիպը|
| supportedExtendedFeatures | bool | Ցույց է տալիս արդյոք տվյալների աղբյուրը կարող է պարունակել SupportedEncoding.Unicode կոդավորմամբ սյուներ։ Լռությամբ արժեքը false է։ 4X-ից այսպիսի հատկությամբ DS-ի օգտագործումը արգելվում է:| 

Լրացման օրինակ՝
```c#
            this.Schema = new Schema(this.Name, ConstantsArmenian.ProcessingMode.ToArmenianANSICached(), ConstantsEnglish.ProcessingMode, typeof(DataRow), typeof(Param));
```

- Հետո անհրաժեշտ է սխեմայում ավելացնել տվյալների աղբյուրի սյուների նկարագրությունները։ Դա արվում է Schema դասի `AddColumn` մեթոդի միջոցով, որը ունի հետևյալ կառուցվածքը՝
```c#
        public void AddColumn(string name, string source, string armenianCaption, string englishCaption, FieldType columnType,
                              bool isPermanent = false, short start = 0, bool autoProcess = true,
                              string armenianDescription = null, string englishDescription = null,
                              FieldType showType = null, short width = 0,
                              short headlines = 2, bool isTrimEnd = false, bool mayNotExistInSQL = false,
                              SupportedEncoding supportedEncoding = SupportedEncoding.ArmenianAnsi)
```
Մեթոդի պարամետրի մասին ամբողջական ինֆորմացիան ներկայացված է ստորև՝

| Անվանում | Տեսակ | **Նկարագրություն** | Լռությամբ արժեք|
| --- | --- | --- | --- |
| name | string | Սյան ներքին անվանում| Չունի, արժեքը պարտադիր է լրացնել |
| source | string | sql-based տվյալների աղբյուրի դեպքում նշվում է SQL-ից կարդացվող սյան անունը, իսկ array-based տվյալների աղբյուրի դեպքում՝ սյան համարը| Չունի, արժեքը պարտադիր է լրացնել |
| armenianCaption | string | Սյան հայերեն անվանում(անվանումը անհրաժեշտ է փոխանցել Ansi կոդավորմամբ)| Չունի, արժեքը պարտադիր է լրացնել |
| englishCaption | string | Սյան անգլերեն անվանում| Չունի, արժեքը պարտադիր է լրացնել | 
| columnType | FieldType | Սյան համակարգային տիպ| Չունի, արժեքը պարտադիր է լրացնել | 
| isPermanent | bool | Սյունը հավերժական է թե ոչ: Ընթացիկ դիտելու ձևից ծրագրային կարելի է կարդալ միայն հավերժական սյունակները։| false |
| start | short | Սահմանում է մեկնարկային դիրքը, որից սկսած ցույց է տալիս արժեք որևէ ձևաչափված դաշտից։ Նախատեսված է fSPEC-ից կամ այլ տողային դաշտերից տվյալը ճիշտ տիպով կարդալու և ցույց տալու համար։ Կարդացվող արժեքի երկարությունը որոշվում է columnType-ից կախված։| 0 |
| autoProcess | bool |Այս հատկության false արժեքի դեպքում սյունը համարվում է հաշվարկային։ Սյան արժեքների հաշվարկը կարելի է իրականացնել գերբեռնելով `ProcessRow` կամ `AfterDataReaderClose` մեթոդը։ Իսկ այս հատկությունը ունեցող սյան համար որպես source կարելի է նշել կամայական տեքստ։| true |
| armenianDescription | string | Սյան հայերեն նկարագրություն(անվանումը անհրաժեշտ է փոխանցել Ansi կոդավորմամբ)| null |
| englishDescription | string | Սյան անգլերեն նկարագրություն | null |
| showType | FieldType | Սահմանում է համակարգային տիպը ցուցադրման ժամանակ։ Եթե այս պարամետրը բացակայում է, ապա օգտագործվում է columnType հատկության արժեքը։ Սովորոբար այս հատկությունը օգտագործում են, եթե տվյալների տիպը, որը համապատասխանում է սյունակի արժեքներին, հարմար չի ցուցադրման համար։ Օրինակ եթե columnType = new StringFieldType(150) է, բայց շատ դեպքերում բավական է տեսնել տողի սկիզբը, ապա կարելի է սահմանել showType = new StringFieldType(32):| null |
| width | short | Սյունակի լայնությունը: Արժեք չփոխանցելու դեպքում որոշվում է կախված սյան armenianCaption, englishCaption, columnType, showType հատկություններից| 0 | 
| headlines | short | Սյան անվանման մեջ տողերի քանակ | 2 | 
| isTrimEnd | bool | Սյան արժեքների վերջից trim են արվում թե ոչ| false |
| mayNotExistInSQL | bool | sql-based տվյալների աղբյուրի sql հարցման մեջ տվյալ սյան արժեքների լրացման համար նախատեսված սյան վերադարձը պարտադիր է թե ոչ։ Սյան արժեքների լրացման համար անհրաժեշտ sql-ական սյան անվանումը նշվում է source դաշտում։| false |
| supportedEncoding | SupportedEncoding | Սյան կոդավորման տեսակը։ Լռությամբ արժեքը false է։ SupportedEncoding-ը կարող է լինել երեք տեսակի՝ ArmenianAnsi, RussionAnsi և Unicode, լռելյան ArmenianAnsi է։ Unicode արժեքի դեպքում անհրաժեշտ է սխեմայի SupportedExtendedFeatures հատկության արժեքը լինի true:| SupportedEncoding.ArmenianAnsi |

Լրացման օրինակ՝
```c#
            this.Schema.AddColumn(nameof(DataRow.CODE), "0", ConstantsArmenian.Code.ToArmenianANSICached(), ConstantsEnglish.Code, FieldTypeProvider.GetNumericFieldType(3), true);
            this.Schema.AddColumn(nameof(DataRow.DESC), "1", ConstantsArmenian.Descript.ToArmenianANSICached(), ConstantsEnglish.Descript, FieldTypeProvider.GetStringFieldType(DSConstantsLength.PrModeDesc), true
                        , showType: FieldTypeProvider.GetStringFieldType(DSConstantsLength.PrModeDescShow));
```

Եթե տվյալների աղբյուրը պարունակում է պարամետրեր, ապա սխեմայում անհրաժեշտ է ավելացնել նաև պարամետրերի նկարագրությունը։
Դա արվում է Schema դասի `AddParam` մեթոդի միջոցով, որը ունի հետևյալ կառուցվածքը՝
```c#
        public void AddParam(string name, string description, FieldType fieldType, string userReportValue = null,
                     long? supportedFilterType = null, bool required = false, string eDescription = "",
                     bool nullable = false, bool allowTime = false)
```

Մեթոդի պարամետրի մասին ամբողջական ինֆորմացիան ներկայացված է ստորև՝

| Անվանում | Տեսակ | **Նկարագրություն** | Լռությամբ արժեք|
| --- | --- | --- | --- |
| name | string | Պարամետրի ներքին անվանում| Չունի, արժեքը պարտադիր է լրացնել | 
| description | string | Պարամետրի հայերեն նկարագրություն(նկարագրությունը անհրաժեշտ է փոխանցել Ansi կոդավորմամբ)| Չունի, արժեքը պարտադիր է լրացնել | 
| fieldType | FieldType | Պարամետրի համակարգային տիպ | Չունի, արժեքը պարտադիր է լրացնել | 
| userReportValue | string | Սահմանում է պարամետրի արժեքը օգտագործողի կողմից նկարագրվող հաշվետվություններում։ Այս արժեքը չի կարող փոփոխվել օգտագործողի կողմից | null |
| supportedFilterType | long? | Եթե պարամետրի տիպը ժառանգ է  ParamValuePair<T> դասից, ապա պետք է նշել պարամետրի ֆիլտրման հասանելի  տիպերը | null |
| required | bool |  Պարամետրի արժեքի լրացումը պարտադիր է թե ոչ| false |
| eDescription | string | Պարամետրի անգլերեն նկարագրություն| string.Empty | 
| nullable | bool | Պարամետրը կարող է ընդունել null տիպի արժեք թե ոչ | false |
| allowTime | bool | Եթե պարամետրի համակարգային տիպը ամսաթվային տիպի է(Date, DateLong, DateRep), ապա ամսաթվի հետ միասին լինի ժամանակը թե ոչ| false |

Լրացման օրինակ՝
```c#
            this.Schema.AddParam(nameof(Param.SourceType), ConstantsArmenian.Type.ToArmenianANSICached(), FieldTypeProvider.GetNumericFieldType(2), eDescription: ConstantsEnglish.Type);
```

Տվյալների աղբյուրի ըստ տվյալների բեռնման աղբյուրի լինում է 2 տեսակի՝ sql-based և array-based:
Տվյալների աղբյուրի տվյալների բեռնման տեսակը որոշվում է IsSQLBased boolean տիպի հատկության միջոցով, որի լռությամբ արժեքը true է։

Եթե տվյալների աղբյուրը array-based է, ապա անհրաժեշտ է գերբեռնել IsSQLBased հատկությունը՝ վերադարձնելով false արժեք ու գերբեռնել `Task FillData(DataSourceArgs<P> args, CancellationToken stoppingToken)` մեթոդը` տվյալների աղբյուրի տվյալները ձևավորելու համար, որտեղ որպես P անհրաժեշտ է փոխանցել տվյալների աղբյուրի պարամետրերը նկարագրող դասը։

Լրացման օրինակ՝
```c#
        protected override Task FillData(DataSourceArgs<Param> args, CancellationToken stoppingToken)
        {
            this.Rows.AddRange(
            [
                new DataRow
                {
                    CODE = 0,
                    DESC = Resources.WithoutService
                },
                new DataRow
                {
                    CODE = 1,
                    DESC = args.Parameters.SourceType == 0 ? Resources.DSProcessingMode1 : Resources.DocTemplateProcessingMode1
                },
                new DataRow
                {
                    CODE = 2,
                    DESC = Resources.RunOnServer
                }
            ]
            );
            return Task.CompletedTask;
        }
```
FillData մեթոդում ստեղծում ենք տվյալների աղբյուրի տողերը,որոնք հետո ավելացնում ենք տվյալների աղբյուրի `Rows` հատկությանը։ Յուրաքանչյուր տող տվյալների աղբյուրի սյուները նկարագրող դասի տիպի է։
