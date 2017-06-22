# Parliament.uk Search.Api

Search API for everything public under the parliament.uk domain.

Currently powering [beta.parliament.uk/search](https://beta.parliament.uk/search).

This project is compliant with the [OpenSearch 1.1 specification](http://www.opensearch.org/Specifications/OpenSearch/1.1).

## Search API endpoint

| Query string parameter | OpenSearch parameter | Required |
| ---------------------- | -------------------- | -------- |
| q                      | searchTerms          | True     |
| start                  | startIndex           | False    |
| count                  | count                | False    |

*e.g.* /search?q=who+is+my+mp&start=1&count=10

## Feed serialisation formats

The search API supports 4 serialisation formats for its response feeds through the **Accept** header.

**Accept** header supported values:

- "application/atom+xml"
- "application/rss+xml"
- "text/xml"
- "application/xml"

## Description API endpoint

The /description endpoint returns a valid OpenSearch description document.

OpenSearch description documents can be used to add a search engine to your browser or file explorer.

## Credits

Authors/contributors to this project in alphabetical order:

* Matthieu Bosquet
* Raphael Leung
* Robert Brook
* Samu Lang
* Wojciech Starwiarski

## Licence

This project is licenced under the [Open Parliament Licence](https://www.parliament.uk/site-information/copyright-parliament/open-parliament-licence/).
