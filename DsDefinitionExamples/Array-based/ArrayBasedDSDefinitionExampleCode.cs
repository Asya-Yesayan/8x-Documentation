using System;
using System.Threading;
using System.Threading.Tasks;
using ArmSoft.AS8X.Common.Extensions;
using ArmSoft.AS8X.Common.FieldTypes;
using ArmSoft.AS8X.Core.Constants;
using ArmSoft.AS8X.Core.DS;
using ArmSoft.AS8X.Core.Properties;

namespace ArmSoft.AS8X.Core.DSImplementation
{
    [DataSource(nameof(ProcMode))]
    public class ProcMode : DataSource<ProcMode.DataRow, ProcMode.Param>
    {
        public class DataRow : IExtendableRow
        {
            public short CODE { get; set; }
            public string DESC { get; set; }
            public object Extend { get; set; }
        }
        public class Param
        {
            public short SourceType { get; set; }
        }
        public override bool IsSQLBased
        {
            get
            {
                return false;
            }
        }
        public ProcMode(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.Schema = new Schema(this.Name, ConstantsArmenian.ProcessingMode.ToArmenianANSICached(), ConstantsEnglish.ProcessingMode, typeof(DataRow), typeof(Param));

            this.Schema.AddColumn(nameof(DataRow.CODE), "0", ConstantsArmenian.Code.ToArmenianANSICached(), ConstantsEnglish.Code, FieldTypeProvider.GetNumericFieldType(3), true);
            this.Schema.AddColumn(nameof(DataRow.DESC), "1", ConstantsArmenian.Descript.ToArmenianANSICached(), ConstantsEnglish.Descript, FieldTypeProvider.GetStringFieldType(DSConstantsLength.PrModeDesc), true
                        , showType: FieldTypeProvider.GetStringFieldType(DSConstantsLength.PrModeDescShow));
            // Parameters
            this.Schema.AddParam(nameof(Param.SourceType), ConstantsArmenian.Type.ToArmenianANSICached(), FieldTypeProvider.GetNumericFieldType(2), eDescription: ConstantsEnglish.Type);
        }

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
    }
}
