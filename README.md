# Parliament.uk Search.Api

Search API for everything public under the parliament.uk domain.

Currently powering [beta.parliament.uk/search](https://beta.parliament.uk/search).

This project is compliant with the [OpenSearch 1.1 specification](http://www.opensearch.org/Specifications/OpenSearch/1.1).

The search API can be configured to use different search result providers.

## Search API endpoint

### Query String Parameters

The search API currently supports one combination of parameters (matching [parameters described in the OpenSearch 1.1 specification](http://www.opensearch.org/Specifications/OpenSearch/1.1#OpenSearch_1.1_parameters)).

| Query string parameter | Required | Validation            | Default | OpenSearch parameter |
|------------------------|----------|-----------------------|---------|----------------------|
| q                      | True     | string                |         | searchTerms          |
| start                  | False    | int, min(1)           | 1       | startIndex           |
| count                  | False    | int, min(1), max(100) | 10      | count                |

*n.b.* Corresponding [OpenSearch URL template](http://www.opensearch.org/Specifications/OpenSearch/1.1#OpenSearch_URL_template_syntax) query string "?q=\{searchTerms\}\&amp;start=\{startPage?\}\&amp;count=\{count?\}".

*e.g.* Example query "/search?q=who+is+my+mp&start=1&count=10".

### Headers

The search API supports 4 feed serialisation formats for its response feeds through the **Accept** header.

| Header | Value |
|--------|-------|
| Accept | "application/atom+xml" or "application/rss+xml" or "text/xml" or "application/xml" |

## Description API endpoint

The /description endpoint returns a valid OpenSearch description document.

OpenSearch description documents can be used to add a search engine to your browser or file explorer.

## Search Engine providers

OpenSearch parameters have corresponding search engine providers parameters. 

### Google CSE

| OpenSearch parameter | Query string parameter | Required | Validation           | Default |
|----------------------|------------------------|----------|----------------------|---------|
| searchTerms          | q                      | True     | string               |         |
| startIndex           | start                  | False    | int, min(0)          | 0       |
| count                | num                    | False    | int, min(1), max(10) | 10      |

*n.b.* [Google CSE will be completely shut down by April 1st 2018](https://cse.google.com/cse/).

### Bing search API

| OpenSearch parameter | Query string parameter | Required | Validation           | Default |
|----------------------|------------------------|----------|----------------------|---------|
| searchTerms          | q                      | True     | string               |         |
| startIndex           | offset                 | False    | int, min(0)          | 0       |
| count                | count                  | False    | int, min(1), max(50) | 10      |


## Credits

Authors/contributors to this project in alphabetical order:

* Matthieu Bosquet
* Raphael Leung
* Robert Brook
* Samu Lang
* Wojciech Starwiarski
