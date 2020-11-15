using Extractor.Extractors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Extractor
{
  public class MultiStream
  {
    public readonly StreamType[] StreamTypes;

    public MultiStream(StreamType[] streamTypes)
    {
      StreamTypes = streamTypes;
    }
  }

  public class StreamType
  {
    public Stream Stream;
    public ExportType ExportType;
  }
}
