.PHONY: build clean

MONOPATH=../../rcr/lge
LGE= ../../rcr/lge/lge.dll

SOURCES = $(wildcard *.cs)
OUT = demo4.exe

COMPILER = mcs
FLAGS = -pkg:dotnet -target:exe -r:$(LGE) -out:$(OUT)

build: $(OUT)

$(OUT): $(SOURCES)
	@ $(COMPILER) $(FLAGS) $(SOURCES)

run: $(OUT)
	(MONO_PATH=$(MONOPATH) mono $(OUT))

clean:
	@ rm $(OUT)
