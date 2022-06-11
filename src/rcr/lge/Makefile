.PHONY: build clean

SOURCES = $(wildcard *.cs)
OUT = lge.dll

COMPILER = mcs
FLAGS = -pkg:dotnet -target:library -out:$(OUT)

build: $(OUT)

$(OUT): $(SOURCES)
	@ $(COMPILER) $(FLAGS) $(SOURCES)

clean:
	@ rm $(OUT)
