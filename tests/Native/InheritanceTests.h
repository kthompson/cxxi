#include "common.h"

class EXPORT NumberClass {
protected:
	int num;
public:
	NumberClass (int n);
	virtual int NegativeNumber () const;
	virtual int Number () const;
	virtual ~NumberClass ();
};

class EXPORT AdderClass : public NumberClass {
public:
	AdderClass (int n);
	virtual void Add (int n);
};
class EXPORT AdderClassWithVirtualBase : public virtual NumberClass {
public:
	AdderClassWithVirtualBase (int n);
	virtual void Add (int n);
};
class EXPORT AdderClassWithVirtualBaseNoVirtualMethods : public virtual NumberClass {
public:
	AdderClassWithVirtualBaseNoVirtualMethods (int n) : NumberClass(n) {}
	void Add (int n) { NumberClass::num += n; }
};

class EXPORT MultiplierClass : public NumberClass {
public:
	MultiplierClass (int n);
	virtual void Multiply (int n);
};
class EXPORT MultiplierClassWithVirtualBase : public virtual NumberClass {
public:
	MultiplierClassWithVirtualBase (int n);
	virtual void Multiply (int n);
};

class EXPORT ClassWithNonVirtualBases : public AdderClass, public MultiplierClass {
public:
	// num is not shared between AdderClass and MultiplierClass; Add and Multiply should operate on different numbers
	ClassWithNonVirtualBases (int addN, int multN) : AdderClass (addN), MultiplierClass (multN) {}
	virtual void CallMultiply (int n) { this->Multiply (n); }
};
class EXPORT ClassWithVirtualBases : public AdderClassWithVirtualBase, public MultiplierClassWithVirtualBase {
public:
	// num is shared between AdderClass and MultiplierClass; Add and Multiply should both operate on n
	ClassWithVirtualBases (int n) : NumberClass (n), AdderClassWithVirtualBase (n-1), MultiplierClassWithVirtualBase (n-2) {}
};


class EXPORT ClassThatOverridesStuff : public NumberClass {
protected:
	int myNum;
public:
	ClassThatOverridesStuff (int num, int my);
	virtual int Number () const;
	virtual ~ClassThatOverridesStuff ();
	virtual int BaseNumber () const;
	static NumberClass* GetInstance (int num, int my);
};

class EXPORT ClassThatRoundtrips : public MultiplierClass {
protected:
	MultiplierClass* that;
public:
	ClassThatRoundtrips (int n, MultiplierClass* managed) : MultiplierClass (n) { this->that = managed; }
	virtual MultiplierClass* GetThat () { return this->that; }
	virtual MultiplierClass* GetThis () { return this; }
};