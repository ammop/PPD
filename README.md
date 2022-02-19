# DTCC PPD
Download DTCC PPD data (CA, CTFCC, and SEC for Commodities, Credits, Equities, Forex, and Rates) in C#

## Source Data

* URL: https://pddata.dtcc.com/gtr/canada/dashboard.do
* Example File: https://kgc0418-tdw-data-0.s3.amazonaws.com/ca/eod/CA_CUMULATIVE_CREDITS_2022_01_30.zip

The files all follow the same format:

Host: https://kgc0418-tdw-data-0.s3.amazonaws.com/

* Source, Lower: ca
* Literal: /eod/
* Source, Upper: CA
* Underscore
* Literal: CUMULATIVE
* Product: CREDITS
* Date: YYYY_MM_DD
* Extension: .zip

Underscores where appropriate

Inside is a single csv with headers and N rows of data

### Current Capabilities

Currently, the program only supports download.  

### Planned Capabilities

* Unzip
* Table Creation (MSSQL)
* Ingestion
