<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:atom="http://www.w3.org/2005/Atom" exclude-result-prefixes="atom">
  <xsl:output method="html"/>
  <xsl:template match="atom:feed">
    <html>
      <head>
        <title>
          <xsl:value-of select="atom:title"/>
        </title>
        <style>
          * {
          font-style: normal;
          font-family: sans-serif;
          padding: 0;
          margin: 0;
          }

          body {
          max-width: 600px;
          padding: 20px;
          }

          form {
          margin-top: 20px;
          }

          ol li {
          margin-top: 20px;
          list-style: none;
          }
          
          h2 {
            font-weight: normal;
          }

          a {
          color: #1a0dab;
          }

          p {
          margin-top: 10px;
          }

          cite {
          display: block;
          margin-top: 10px;
          color: #006621;
          font-size: small;
          }

          ul {
          margin-top: 10px;
          }

          ul li {
          display: inline;
          padding: 2px 4px;
          background: silver;
          }
        </style>
        <script>
          window.addEventListener("load", function() {
          document.getElementById("q").value = window.location.search.split("?")[1].split("=")[1];
          });
        </script>
      </head>
      <body>
        <h1>
          <xsl:value-of select="atom:title"/>
        </h1>
        <form method="get">
          <label for="q">search term</label>
          <input id="q" name="q"></input>
          <input type="submit"></input>
        </form>
        <xsl:apply-templates select="." mode="entries"/>
      </body>
    </html>
  </xsl:template>
  <xsl:template match="atom:feed[atom:entry]" mode="entries">
    <ol>
      <xsl:apply-templates select="atom:entry"/>
    </ol>
  </xsl:template>
  <xsl:template match="atom:feed" mode="entries">
    <p>No results</p>
  </xsl:template>
  <xsl:template match="atom:entry">
    <li>
      <h2>
        <a href="{atom:link/@href}">
          <xsl:value-of select="atom:title" disable-output-escaping="yes"/>
        </a>
      </h2>
      <xsl:apply-templates select="hints[hint]"/>
      <cite>
        <xsl:value-of select="atom:link/@title" disable-output-escaping="yes"/>
      </cite>
      <p>
        <xsl:value-of select="atom:content" disable-output-escaping="yes"/>
      </p>
    </li>
  </xsl:template>
  <xsl:template match="hints">
    <ul>
      <xsl:apply-templates select="hint"/>
    </ul>
  </xsl:template>
  <xsl:template match="hint">
    <li>
      <xsl:value-of select="Name"/>
    </li>
  </xsl:template>
</xsl:stylesheet>
