.PHONY: build clean

MONOPATH=../../rcr/lge
LGE= ../../rcr/lge/lge.dll
RESOURCES=../resources

SOURCES = $(wildcard *.cs)
OUT = pong.exe

COMPILER = mcs
FLAGS = -pkg:dotnet -target:exe -r:$(LGE) -out:$(OUT)

build: $(OUT)

$(OUT): $(SOURCES)
	@ $(COMPILER) $(FLAGS) $(SOURCES)

run: $(OUT)
	(MONO_PATH=$(MONOPATH) mono $(OUT) $(RESOURCES))

clean:
	@ rm $(OUT)
