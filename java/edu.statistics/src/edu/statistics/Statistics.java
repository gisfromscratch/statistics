package edu.statistics;

import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.io.LineNumberReader;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.StringTokenizer;

import org.apache.commons.math3.stat.Frequency;

public class Statistics {

	public static void main(String[] arguments) throws IOException {
		for (String argument : arguments) {
			LineNumberReader reader = new LineNumberReader(new FileReader(new File(argument)));
			try {
				System.out.println(String.format("Reading %s", argument));
				List<String> fieldNames = new ArrayList<String>();
				Map<String, Frequency> frequencies = new HashMap<String, Frequency>();
				boolean readHeader = true;
				String line;
				while (null != (line = reader.readLine())) {
					StringTokenizer tokenizer = new StringTokenizer(line, ",");
					for (int tokenIndex = 0; tokenizer.hasMoreTokens(); tokenIndex++) {
						String nextToken = tokenizer.nextToken();
						if (readHeader) {
							fieldNames.add(nextToken);
							frequencies.put(nextToken, new Frequency());
						} else {
							if (fieldNames.size() <= tokenIndex) {
								break;
							}
							
							String fieldName = fieldNames.get(tokenIndex);
							if (frequencies.containsKey(fieldName)) {
								Frequency frequency = frequencies.get(fieldName);
								frequency.addValue(nextToken);
							}
						}
					}
					if (readHeader) {
						readHeader = fieldNames.isEmpty();
					}
				}
				
				for (Map.Entry<String, Frequency> fieldEntry : frequencies.entrySet()) {
					String fieldName = fieldEntry.getKey();
					Frequency frequency = fieldEntry.getValue();
					System.out.println(String.format("%s\t", fieldName));
					List<Comparable<?>> modeValues = frequency.getMode();
					for (Comparable<?> modeValue : modeValues) {
						System.out.println(String.format("\t%s:\t%d", modeValue, frequency.getCount(modeValue)));
					}
				}
				System.out.println();
			}
			finally {
				reader.close();
			}
		}
	}
}
