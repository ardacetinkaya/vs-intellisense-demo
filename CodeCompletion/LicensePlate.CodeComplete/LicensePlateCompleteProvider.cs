namespace LicensePlateCodeComplete
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Completion;
    using Microsoft.CodeAnalysis.Options;
    using Microsoft.CodeAnalysis.Text;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;

    //Export this completion prvider for Visual Studio extension model
    [ExportCompletionProvider("LicensePlateCompleteProvider", LanguageNames.CSharp)]
    internal class LicensePlateCompleteProvider : CompletionProvider
    {
        private const string StartKey = nameof(StartKey);
        private const string LengthKey = nameof(LengthKey);
        private const string NewTextKey = nameof(NewTextKey);

        private List<LicensePlate> LicensePlates { get; set; }

        public LicensePlateCompleteProvider()
        {

        }



        public override bool ShouldTriggerCompletion(SourceText text, int caretPosition, CompletionTrigger trigger, OptionSet options)
        {
            //Check to trigger completion
            //Only if trigger is Insertion and " is must be inserted as trigger character
            if (trigger.Kind == CompletionTriggerKind.Insertion)
            {
                if (trigger.Character == '"')
                {
                    return true;
                }
            }

            return false;
        }

        public override async Task ProvideCompletionsAsync(CompletionContext context)
        {
            //Do not provide completion with ctrl+space
            if (context.Trigger.Kind == CompletionTriggerKind.InvokeAndCommitIfUnique)
                return;

            var document = context.Document;
            var cancellationToken = context.CancellationToken;
            var startPosition = context.Position;

            //Get code content
            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            //Find exact starting position by checking code syntax
            while (char.IsLetter(text[startPosition - 1]))
            {
                startPosition--;
            }


            var replacmentSpan = TextSpan.FromBounds(startPosition, context.Position);

            FillLicenses(replacmentSpan);

            //Fill intelliSense dialog box with suggested data
            foreach (var item in LicensePlates)
            {
                var textChange = item.Change.TextChange;
                var properties = ImmutableDictionary.CreateBuilder<string, string>();
                properties.Add(StartKey, textChange.Span.Start.ToString());
                properties.Add(LengthKey, textChange.Span.Length.ToString());
                properties.Add(NewTextKey, textChange.NewText);

                context.AddItem(CompletionItem.Create(
                        displayText: item.City,
                        inlineDescription: item.Code,
                        properties: properties.ToImmutable()));
            }

            context.IsExclusive = true;
        }

        public override Task<CompletionChange> GetChangeAsync(Document document, CompletionItem item, char? commitKey, CancellationToken cancellationToken)
        {
            // These values have always been added by us.
            var startString = item.Properties[StartKey];
            var lengthString = item.Properties[LengthKey];
            var newText = item.Properties[NewTextKey];

            return Task.FromResult(CompletionChange.Create(new TextChange(new TextSpan(int.Parse(startString), int.Parse(lengthString)), newText)));
        }

        private void FillLicenses(TextSpan replacmentSpan)
        {
            LicensePlates = new List<LicensePlate>
            {
                //Some cities and their license plate codes
                new LicensePlate("Adana","01",replacmentSpan),
                new LicensePlate("Adıyaman","02",replacmentSpan),
                new LicensePlate("Afyon","03",replacmentSpan),
                new LicensePlate("Ağrı","04",replacmentSpan),
                new LicensePlate("Amasya","05",replacmentSpan),
                new LicensePlate("Ankara","06",replacmentSpan),
                new LicensePlate("Antalya","07",replacmentSpan),
                new LicensePlate("Artvin","08",replacmentSpan),
                new LicensePlate("Aydın","09",replacmentSpan),
                new LicensePlate("Balıkesir","10",replacmentSpan),
                new LicensePlate("Bilecik","11",replacmentSpan),
                new LicensePlate("Bingöl","12",replacmentSpan),
                new LicensePlate("Bitlis","13",replacmentSpan),
                new LicensePlate("Bolu","14",replacmentSpan),
                new LicensePlate("Burdur","15",replacmentSpan),
                new LicensePlate("Bursa","16",replacmentSpan),
                new LicensePlate("Çanakkale","17",replacmentSpan),
                new LicensePlate("Çankırı","18",replacmentSpan),
                new LicensePlate("Çorum","19",replacmentSpan),
                new LicensePlate("Denizli","20",replacmentSpan),
                new LicensePlate("Diyarbakır","21",replacmentSpan),
                new LicensePlate("Edirne","22",replacmentSpan),
                new LicensePlate("Elazığ","23",replacmentSpan),
                new LicensePlate("Erzincan","24",replacmentSpan),
                new LicensePlate("Erzurum","25",replacmentSpan),
                new LicensePlate("Eskişehir","26",replacmentSpan),
                new LicensePlate("Gaziantep","27",replacmentSpan),
                new LicensePlate("Giresun","28",replacmentSpan),
                new LicensePlate("Gümüşhane","29",replacmentSpan),
                new LicensePlate("Hakkari","30",replacmentSpan),
                new LicensePlate("Hatay","31",replacmentSpan),
                new LicensePlate("Isparta","32",replacmentSpan),
                new LicensePlate("İçel","33",replacmentSpan),
                new LicensePlate("İstanbul","34",replacmentSpan),
                new LicensePlate("İzmir","35",replacmentSpan),
                new LicensePlate("Kars","36",replacmentSpan),
                new LicensePlate("Kastamonu","37",replacmentSpan),
                new LicensePlate("Kayseri","38",replacmentSpan),
                new LicensePlate("Kırklareli","39",replacmentSpan),
                new LicensePlate("Kırşehir","40",replacmentSpan),
                new LicensePlate("Kocaeli","41",replacmentSpan)
            };
        }


    }

    internal class LicensePlate
    {
        public string City { get; private set; }
        public string Code { get; private set; }
        public readonly CompletionChange Change;
        public LicensePlate(string city, string code, TextSpan replacmentSpan)
        {
            City = city;
            Code = code;
            Change = CompletionChange.Create(new TextChange(replacmentSpan, code));
        }
    }
}
