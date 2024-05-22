# Տվյալների աղբյուրի կատարում
Կից ներկայացված է "SYSDEF" տվյալների աղբյուրի կատարման օրինակը՝ օգտագործելով ArmSoft.AS8X.Client գրադարանը։
* Տվյալների աղբյուրը կատարելու համար անհրաժեշտ է նախ ստանալ նկարագրութունը DefinitionsCache service-ի GetDataSourceDefinition մեթոդով, որին անհրաժեշտ է փոխանցել տվյալների աղբյուրի ներքին անվանումը։

``` C#
var dsDefinition = Settings.ServiceProvider.GetRequiredService<DefinitionsCache>().GetDataSourceDefinition("SYSDEF");
```

* Հետո անհրաժեշտ է ստեղծել տվյալների աղբյուրի պարամետրերը նկարագրող դասի instance, որի Definition հատկությանը անհրաժեշտ է փոխանցել տվյալների աղբյուրի նկարագրության Parameters դաշտը ու պարամետրերին էլ փոխանցել համապատասխան արժեքները։

``` C#
var parameters = new SysDefParameters
            {
                Definition = dsDefinition.Parameters, //պարտադիր լրացման
                DefType = "0"
            };
```

* Հետո ստեղծում ենք DataSource դասի instance ` DataSource<R,P>, որտեղ որպես R փոխանցում ենք տվյալների աղբյուրի սյունակները նկարագրող դասը, որպես P փոխանցում ենք տվյալների աղբյուրի պարամետրերը նկարագրող դասը։

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

* Տվյալների աղբյուրը կարճ եղանակով կատարելու համար անհրաժեշտ է DataSource<R,P> դասի instance-ի Execute մեթոդը,որին պարտադիր է փոխանցել տվյալների աղբյուրի պարամետրերը։

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

* Տվյալների աղբյուրը երկար եղանակով կատարելու համար անհրաժեշտ է DataSource<R,P> դասի instance-ի Execute մեթոդը,որին պարտադիր է փոխանցել տվյալների աղբյուրի պարամետրերը։
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

