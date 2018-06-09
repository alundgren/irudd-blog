declare global {
    namespace Markdown {
        class Converter {
            makeHtml(input: string) : string;
            hooks : IMarkdownHooks
        }

        interface IMarkdownHooks {
            chain(name: string, callback: ((text: string, rgb: any) => void))
        }
    }
}

export { };