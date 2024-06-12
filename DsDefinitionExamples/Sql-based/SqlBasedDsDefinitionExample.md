# Ինչպես նկարագրել sql-based տվյալների աղբյուր

- Անհրաժեշտ է հայտատարել դաս, որը ժառանգում է DataSource<R, P> դասը, որտեղ որպես R անհրաժեշտ է փոխանցել տվյալների աղբյուրի սյուները նկարագրող դաս, իսկ որպես P` տվյալների աղբյուրի պարամետրերը նկարագրող դաս։

```c#
    public class DocCap : DataSource<DocCap.DataRow, DocCap.Param>
```

Եթե տվյալների աղբյուրը չի պարունակում պարամետրեր, ապա որպես P անհրաժեշտ է փոխանցել NoParam դասը։
```c#
    public class DocCap : DataSource<DocCap.DataRow, NoParam>
```

- Տվյալների աղբյուրի սյուները նկարագրող դասը պարտադիր պետք է իմպլեմենտացնի `IExtendableRow` ինտերֆեյսը։ Այդ դասում անհրաժեշտ է որպես հատկություններ ավելացնել տվյալների աղբյուրի սյուները։
```c#
        public class DataRow : IExtendableRow
        {
            public string DocType { get; set; }
            public string fCAPTION { get; set; }
            public object Extend { get; set; }
        }
```

- Տվյալների աղբյուրի պարամետրերը նկարագրող դասում նույնպես պետք է որպես հատկություններ պետք է ավելացնել պարամետրը։
```c#
        public class Param
        {
            public string DocType { get; set; }
        }
```

- Հետո անհրաժեշտ է ձևավորել տվյալների աղբյուրի կոնստրուկտորը, որը իր հերթին պիտի կանչի base DataSource<R, P> դասի կոնստրուկտորը: Կոնստուկտորում անհրաժեշտ է inject անել բոլոր այն service-ները, որոնք հետագայում հարկավոր են լինելու աշխատանքի համար։
Տվյալների աղբյուրի կոնստրուկտորում պարտադիր պետք է ունենալ IServiceProvider տիպի պարամետր, որն էլ պետք է փոխանցել base դասի կոնստրուկտորին։
```c#
        public readonly IDBService dbService;
        public DocCap(IDBService dbService, IServiceProvider serviceProvider) : base(serviceProvider)
        {
           this.dbService = dbService;
       }
```
Կոնստրուկտորում պետք է նաև ձևավորել տվյալների աղբյուրի սխեման, որը պարունակում է ամբողջական ինֆորմացիա տվյալների աղբյուրի սյուների ու պարամետրերի կառուցվածքի մասին։

base դասի Schema հատկության անհրաժեշտ է վերագրել Schema դասի նոր օբյեկտ։
Schema դասի կոնստրուկտորը ունի հետևյալ շարահյուսությունը՝
```c#
        public Schema(string name, string armenianCaption, string englishCaption, Type rowType, Type paramType,
                      bool supportedExtendedFeatures = false)
```
Կոնստրուկտորի պարամետրերի մասին ամբողջական ինֆորմացիան ներկայացված է ստորև՝

| Անվանում | Տեսակ | **Նկարագրություն** |
| --- | --- | --- |
| name | string | Սխեմայի ներքին անվանում| 
| armenianCaption | string | Սխեմայի հայերեն անվանում(անվանումը անհրաժեշտ է փոխանցել Ansi կոդավորմամբ)| 
| englishCaption | string | Սխեմայի անգլերեն անվանում| 
| rowType | Type | Տվյալների աղբյուրի սյուները նկարագրող դասի տիպը| 
| paramType | Type | Տվյալների աղբյուրի պարամետրերը նկարագրող դասի տիպը| 
| supportedExtendedFeatures | bool | Ցույց է տալիս թե տվյալների աղբյուրը կարող է պարունակել Unicode կոդավորմամբ սյուներ թե ոչ։ Այս հատկությամբ տվյալների աղբյուրի օգտագործումը արգելվում է 4X-ից։ Լռությամբ արժեքը false է։| 

Լրացման օրինակ՝
```c#
this.Schema = new Schema(this.Name, ConstantsArmenian.ParamLog.ToArmenianANSICached(), ConstantsEnglish.ParamLog, typeof(DataRow), typeof(Param));
```

- Հետո անհրաժեշտ է սխեմայում ավելացնել տվյալների աղբյուրի սյուների նկարագրությունները։ Դա արվում է Schema դասի `AddColumn` մեթոդի միջոցով, որը ունի հետևյալ շարահյուսությունը՝
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
            this.Schema.AddColumn(nameof(DataRow.DocType), "DocType", ConstantsArmenian.DocType.ToArmenianANSICached(), ConstantsEnglish.DocType, FieldTypeProvider.GetStringFieldType(SYSDEF.DocNameLength));

            this.Schema.AddColumn(nameof(DataRow.fCAPTION), "DocTypeName", ConstantsArmenian.Name.ToArmenianANSICached(), ConstantsEnglish.Name, FieldTypeProvider.GetStringFieldType(SYSDEF.fCAPTIONLength));
```

Եթե տվյալների աղբյուրը պարունակում է պարամետրեր, ապա սխեմայում անհրաժեշտ է ավելացնել նաև պարամետրերի նկարագրությունը։
Դա արվում է Schema դասի `AddParam` մեթոդի միջոցով, որը ունի հետևյալ շարահյուսությունը՝
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
            this.Schema.AddParam(nameof(Param.DocType), ConstantsArmenian.DocType.ToArmenianANSICached(), FieldTypeProvider.GetStringFieldType(DSConstantsLength.DocCapDocType), eDescription: ConstantsEnglish.DocType);
```

Տվյալների աղբյուրի ըստ տվյալների բեռնման աղբյուրի լինում է 2 տեսակի՝ sql-based և array-based:
Տվյալների աղբյուրի տվյալների բեռնման տեսակը որոշվում է IsSQLBased boolean տիպի հատկության միջոցով, որի լռությամբ արժեքը true է։

Եթե տվյալների աղբյուրը sql-based է(այսինքն տվյալների աղբյուրի տվյալները ստացվում են sql հարցման միջոցով), ապա sql հարցումը ձևավորելու համար անհրաժեշտ է override անել `Task<SqlCommand> MakeSQLCommand(DataSourceArgs<P> args, CancellationToken stoppingToken)` մեթոդը, որտեղ որպես P անհրաժեշտ է փոխանցել տվյալների աղբյուրի պարամետրերը նկարագրող դասը։

```c#
        protected override Task<SqlCommand> MakeSQLCommand(DataSourceArgs<Param> args, CancellationToken stoppingToken)
        {
            using var cmd = this.dbService.Connection.CreateCommand();
            cmd.CommandText = $"""
                               SELECT fNAME AS DocType
                                       , {(LanguageService.IsArmenian ? "fCAPTION" : "fECAPTION")} AS DocTypeName
                               FROM SYSDEF
                               WHERE fSYSTYPE = 0
                               """;
            if (!string.IsNullOrWhiteSpace(args.Parameters.DocType))
            {
                cmd.CommandText += " AND fNAME IN (SELECT item FROM asf_Split_to_table(@DocType, default))";
                cmd.Parameters.Add("@DocType", SqlDbType.VarChar).Value = args.Parameters.DocType;
            }
            return Task.FromResult(cmd);
        }
```
MakeSQLCommand անհրաժեշտ է ունենալ [SqlCommand](https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlcommand?view=netframework-4.8.1) դասի օբյեկտ, որի մեջ կլրացվի հարցումը։

Դա կարելի է անել IDBService դասի Connection հատկության [CreateCommand](https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnection.createcommand?view=netframework-4.8.1&viewFallbackFrom=dotnet-plat-ext-8.0) մեթոդի միջոցով, որը ընթացիկ sql connection-ի համար բացում է [SqlCommand](https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlcommand?view=netframework-4.8.1),որում էլ ձևավորվելու է sql հարցումը։

Հետո ստեղծված SqlCommand դասի օբյեկտի CommandText հատկությանը հարկավոր է փոխանցել հարցման տեքստը։
Եթե տվյալների աղբյուրը պարունակում է պարամետրեր, ապա sql հարցման մեջ չի թույլատրվում միանգամից ավելացնել այդ պարամետրերը։
Այդ պարամետրերը ավելացնելու համար անհրաժեշտ է @-ով ավելացնել պարամետրի անունը, հետո ստեղծված SqlCommand դասի օբյեկտի Parameters հատկությանը [Add](https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlparametercollection.add?view=netframework-4.8.1#system-data-sqlclient-sqlparametercollection-add(system-string-system-data-sqldbtype)) մեթոդը կանչել, որտեղ պետք է փոխանցել պարամետրի անունը ու sql-ական տվյալի տիպը։

Լրացման օրինակ՝
```c#
            if (!string.IsNullOrWhiteSpace(args.Parameters.DocType))
            {
                cmd.CommandText += " AND fNAME IN (SELECT item FROM asf_Split_to_table(@DocType, default))";
                cmd.Parameters.Add("@DocType", SqlDbType.VarChar).Value = args.Parameters.DocType;
            }
```

Ու MakeSQLCommand մեթոդի վերջում անհրաժեշտ է վերադարձնել ձևավորված sql հարցումը։
