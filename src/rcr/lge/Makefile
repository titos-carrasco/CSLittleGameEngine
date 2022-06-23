.PHONY: build clean

SOURCES = $(wildcard *.cs)
OUT = lge.dll
DOC = lge.xml

COMPILER = mcs
FLAGS = -pkg:dotnet -target:library -out:$(OUT) -doc:$(DOC) -define:LINUX

build: $(OUT)

$(OUT): $(SOURCES)
	@ $(COMPILER) $(FLAGS) $(SOURCES)

clean:
	@ rm $(OUT) $(DOC)
