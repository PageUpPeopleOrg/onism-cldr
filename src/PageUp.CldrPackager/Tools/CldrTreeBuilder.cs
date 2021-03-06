using PageUp.CldrPackager.Extensions;

namespace PageUp.CldrPackager.Tools
{
    internal class CldrTreeBuilder
    {
        private readonly IdentifierDictionary<string> values;

        public CldrTree Tree { get; }

        public CldrTreeBuilder()
        {
            this.values = new IdentifierDictionary<string>();
            this.Tree = new CldrTree();
        }

        public void Add(CldrJson cldrJson)
        {
            if (cldrJson == null)
                return;

            foreach (var leaf in cldrJson.Data.Leaves())
            {
                var value = (string) leaf;
                int valueId;

                // TODO refactor this
                if (this.values.HasId(value))
                {
                    valueId = this.values.GetId(value);
                }
                else
                {
                    valueId = this.values.GetId(value);
                    this.Tree.Values.Add(value);
                }
                
                this.Tree.Add(cldrJson.Locale, leaf.Path, valueId);
            }
        }
    }
}