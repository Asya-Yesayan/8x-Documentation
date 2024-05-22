# Տվյալների աղբյուրի կատարում
Կից ներկայացված է "SYSDEF" տվյալների աղբյուրի կատարման օրինակը՝ օգտագործելով ArmSoft.AS8X.Client գրադարանը։
* Տվյալների աղբյուրը կատարելու համար անհրաժեշտ է նախ ստանալ նկարագրութունը DefinitionsCache service-ի GetDataSourceDefinition մեթոդով, որին անհրաժեշտ է փոխանցել տվյալների աղբյուրի ներքին անվանումը։

``` C#
var dsDefinition = Settings.ServiceProvider.GetRequiredService<DefinitionsCache>().GetDataSourceDefinition("SYSDEF");
```

* Հետո անհրաժեշտ է ստեղծել տվյալների աղբյուրի պարամետրերը նկարագրող դասի օբյեկտ, որի Definition հատկությանը անհրաժեշտ է փոխանցել տվյալների աղբյուրի նկարագրության Parameters դաշտը ու պարամետրերին էլ փոխանցել համապատասխան արժեքները։
``` C#
var parameters = new SysDefParameters
            {
                Definition = dsDefinition.Parameters, //պարտադիր լրացման
                DefType = "0"
            };
```
  
  * Տվյալների աղբյուրի պարամետրերի նկարագրման 2 եղանակ կա՝ տիպիզացված և չպիտիզացված։
Չտիպիզացված ձևով նկարագրելու համար անհրաժեշտ է ստեղծել  ParameterCollection դասի օբյեկտ, որի Definition հատկությանը անհրաժեշտ է փոխանցել տվյալների աղբյուրի նկարագրության։
Իսկ պարամետրերին արժեքները տալու համար անհրաժեշտ է պարամետրի անունով ինդեքսատորը վերագրել համապատասխան արժեքը։Արժեքը կարող է լինել կամայական տիպի՝ object:
  Parameters
``` C#
var parameters = new ParameterCollection() { Definition = dsDefinition.Parameters };
parameters["DefType"] = 1;
parameters["DefName"] = "Absence";
```

Տիպիզացված ձևով նկարագրելու համար անհրաժեշտ է սահմանել դաս որը ժառանգում է ParameterCollection դասը։
Այդ դասում որպես հատկություններ սահմանել տվյալների աղբյուրի պարամետրերը։ Հատկությունները պետք է սահմանել հատուկ կառուցվածքով՝
* get-ում անհրաժեշտ է վերադարձնել պարամետրի արժեքը, որը անելու համար անհրաժեշտ է base դասի indexer ին տալ պարամետրի անվանումը ու այդ արժեքը բերել հատկության արժեքի տիպի։
* set-ում պարամետրին արժեքը վերագրելու համար անհրաժեշտ է base դասի indexer ին տալ պարամետրի անվանումը ու նրան վերագրել պարամետրի արժեքը։

```C#
    public class SysDefParameters : ParameterCollection
    {
        public string DefType
        {
            get { return base[nameof(this.DefType)].ToString(); }
            set { base[nameof(this.DefType)] = value; }
        }

        public string DefName
        {
            get { return base[nameof(this.DefName)].ToString(); }
            set { base[nameof(this.DefName)] = value; }
        }
    }
```
Ուրիշ տիպի պարամետեր սահմանելու համար անհրաժեշտ է string-ի փոխարեն գրել պարամետրի համապատսխան տիպը, get-ում ToString()-ը փոխարինել համապատսխան տիպի բերումով ու պարամետրի անունը փոխարինել ձեր պարամետրի անունով։

* Հետո ստեղծում ենք DataSource դասի օբյեկտ` DataSource<R,P>, որտեղ որպես R փոխանցում ենք տվյալների աղբյուրի սյունակները նկարագրող դասը, որպես P փոխանցում ենք տվյալների աղբյուրի պարամետրերը նկարագրող դասը։

``` C#
var ds = new DataSource<SysDefDataRow, SysDefParameters>
            {
                Definition = dsDefinition,
                EncodeResultUnicode = true,
                Client = Settings.CreateApiClient(),
                FirstFetchSize = 100,
                FetchSize = 5000
            };
```
որի կոնստրուկտորում անհրաժեշտ է փոխանցել հետևյալ պարամետրերը՝
* Definition - տվյալների աղբյուրի նկարագրություն,
* EncodeResultUnicode  - տվյալների աղբյուրների տվյալների կոդավորման տեսակ՝ true-ի դեպքում Unicode, false-ի դեպքում Ansi
* Client - Api Client, որի օգնությամբ հարցումներ ենք ուղարկում սերվիս,
* FirstFetchSize - տվյալների աղբյուրը բեռնվում է բլոկ-բլոկ։ Պարամետրը նկարագրում է առաջին բլոկով բեռնվող տողերի քանակը,
* FetchSize - յուրաքանչյուր բլոկով բեռնվող տողերի քանակը(բացի առաջինից)։

Ու տվյալների աղբյուրը կատարելու 2 տարբերակ ունենք՝ կարճ և երկար։

* Տվյալների աղբյուրը կարճ եղանակով կատարելու համար անհրաժեշտ է DataSource<R,P> դասի օբյեկտ-ի Execute մեթոդը,որին պարտադիր է փոխանցել տվյալների աղբյուրի պարամետրերը։

``` C#
var dsResult = ds.Execute(parameters);
```
Մեթոդը ունի հետևյալ signature-ը՝

``` C#
Execute(P param, HashSet<string> columns = default, string isn = null, TimeSpan? timeout = null)
```
Կից ներկայացված է մեթոդի պարամետրերի նկարագրությունը՝
| Անվանում | Տեսակ | **Նկարագրություն** |
| --- | --- | --- |
| param | P | Տվյալների աղբյուրի  պարամետրեր|
| columns | Hashset&lt;string&gt; | Տվյալների աղբյուրի սյուների անվանումների ցուցակ |
| isn | string | Այն սյունակի ներքին անունը, որում լինում է փաստաթղթի ներքին նույնականացման համար |
| timeout | TimeSpan? | timeout |

* Տվյալների աղբյուրը երկար եղանակով կատարելու համար անհրաժեշտ է DataSource<R,P> դասի օբյեկտ-ի Execute մեթոդը,որին պարտադիր է փոխանցել տվյալների աղբյուրի պարամետրերը։
``` C#
dsResult = ds.LongExecute(parameters);
```
Մեթոդը ունի հետևյալ signature-ը՝

``` C#
LongExecute(P param, HashSet<string> columns = default, string isn = null, bool handleEvents = false, TimeSpan? timeout = null)
```
Կից ներկայացված է մեթոդի պարամետրերի նկարագրությունը՝
| Անվանում | Տեսակ | **Նկարագրություն** |
| --- | --- | --- |
| param | P | Տվյալների աղբյուրի  պարամետրեր|
| columns | Hashset&lt;string&gt; | Տվյալների աղբյուրի սյուների անվանումների ցուցակ |
| isn | string | Այն սյունակի ներքին անունը, որում լինում է փաստաթղթի ներքին նույնականացման համար |
| timeout | TimeSpan? | timeout |

