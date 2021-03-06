top_srcdir = ../
BUILD_DIR = $(top_srcdir)/bin/Debug

INTEROP_DLL = \
	$(BUILD_DIR)/Mono.Cxxi.dll

TEST_DLL = $(BUILD_DIR)/Test.dll

NATIVE = \
	ArrayTests \
	MarshalingTests \
	InheritanceTests \
	FieldTests \
	ManglingTests \
	TemplateTests

MANAGED =  \
	ArrayTests.cs \
	FieldTests.cs \
	InheritanceTests.cs \
	ManglingTests.cs \
	MarshalingTests.cs \
	TemplateTests.cs
	
REFERENCES =  \
	-pkg:mono-nunit

NATIVE_SRC = \
 	$(addprefix Native/,$(NATIVE))

all: $(TEST_DLL)

%.cpp: %.h

%.xml: $(addsuffix .h,$(NATIVE_SRC))
	$(GCCXML) -fxml=$@ --gccxml-cxxflags -c Native/$*.h

$(BUILD_DIR)/libTest.so: $(addsuffix .cpp,$(NATIVE_SRC))
	$(CXX) -fPIC --shared -o $@ $^

$(BUILD_DIR)/libTest-inline.so: $(addsuffix .cpp,$(NATIVE_SRC))
	$(CXX) -fPIC --shared -fkeep-inline-functions -o $@ $^

generated: $(addsuffix .xml,$(NATIVE))
	$(foreach X,$?, \
		mono --debug $(BUILD_DIR)/generator.exe -o=$@ -ns=Tests -lib=Test -inline=surrogatelib $(X) && \
	) \
	echo Bindings generated successfully.

$(TEST_DLL): generated $(MANAGED) $(BUILD_DIR)/libTest.so $(BUILD_DIR)/libTest-inline.so
	$(GMCS) -debug -out:$@ -target:library -unsafe $(REFERENCES) -r:$(INTEROP_DLL) generated/*.cs $(MANAGED)

clean:
	$(RM) -rf $(TEST_DLL) generated $(BUILD_DIR)/libTest.so $(BUILD_DIR)/libTest-inline.so $(addsuffix .xml,$(NATIVE))

run: $(TEST_DLL)
	nunit-console -nologo $(TEST_DLL)
