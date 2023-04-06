# Certificates

## Create Private Key

```cli
openssl genrsa -des3 -out demosite.local.key 2048

pass: welkom-1
```

This creates a .key file. If we want our private key unencrypted, we can simply remove the -des3 option from the command.

## Create CSR

If we want our certificate signed, we need a certificate signing request (CSR). The CSR includes the public key and some additional information (such as organization and country).

After these steps are done we have a .csr and a .key file. The latter one is for us, don't share it with others. The .csr is sent and signed by a (trusted) Certificate Authority.

### Via wizard

This prompts you to enter required data.

```cli
openssl req -new \
-key demosite.local.key \
-out demosite.local.csr
```

### With pre-made config

Easier is to use a config (.cnf) file:

```config
[ req ]
default_bits = 2048
req_extensions = req_ext
distinguished_name = req_distinguished_name
prompt = no

[ req_distinguished_name ]
countryName = NL
stateOrProvinceName = Utrecht
localityName = Utrecht
organizationName = One Fox
organizationalUnitName = OFX
commonName = demosite.local
emailAddress = noreply@onefox.nl

[ req_ext ]
subjectAltName = @alt_names

[alt_names]
DNS.1 = demosite.local
DNS.2 = certsite.local
```

And use it like so:

```cli
openssl req -new \
-key demosite.local.key \
-out demosite.local.csr \
-config config.cnf
```

An important field is “Common Name,” which should be the exact Fully Qualified Domain Name (FQDN) of our domain.

“A challenge password” and “An optional company name” can be left empty.

### Create private key and CSR together

We can also create both the private key and CSR with a single command:

```cli
openssl req -new \
-newkey rsa:2048 -nodes \
-keyout demosite.local.key \
-out demosite.local.csr \
-config config.cnf
```

## Creating a Self-Signed Certificate

A self-signed certificate is a certificate that's signed with its own private key. It can be used to encrypt data just as well as CA-signed certificates, but our users will be shown a warning that says the certificate isn't trusted.

Let's create a self-signed certificate (demosite.local.crt) with our existing private key and CSR:

### With premade CSR and KEY

```cli
openssl x509 \
-signkey demosite.local.key \
-in demosite.local.csr \
-req \
-days 365 \
-out demosite.local.crt
```

The -days option specifies the number of days that the certificate will be valid.

We can create a self-signed certificate with just a private key:

```cli
openssl req \
-key demosite.local.key \
-new -x509 \
-days 365 \
-out demosite.local.crt
```

### Temporary CSR

We can even create a private key and a self-signed certificate with just a single command:

```cli
openssl req \
-newkey rsa:2048 \
-keyout demosite.local.key \
-x509 \
-days 365 \
-config config.cnf \
-out demosite.local.crt
```

## Creating a CA-Signed Certificate With Our Own CA

We can be our own certificate authority (CA) by creating a self-signed root CA certificate, and then installing it as a trusted certificate in the local browser.

### Create a Self-Signed Root CA

Create a private key (rootCA.key) and a self-signed root CA certificate (rootCA.crt)

```cli
openssl req \
-x509 \
-sha256 \
-days 1825 \
-newkey rsa:2048 \
-keyout rootCA.key \
-out rootCA.crt
```

Create a configuration text-file (demosite.local.ext) with the following content:

```txt
authorityKeyIdentifier=keyid,issuer
basicConstraints=CA:FALSE
subjectAltName = @alt_names

[alt_names]
DNS.1 = demosite.local
```

The “DNS.1” field should be the domain of our website.

Then we can sign our CSR (domain.csr) with the root CA certificate and its private key:

```cli
openssl x509 \
-req \
-CA rootCA.crt \
-CAkey rootCA.key \
-in demosite.local.csr \
-out demosite.local.crt \
-days 365 \
-CAcreateserial \
-extfile demosite.local.ext
```

As a result, the CA-signed certificate will be in the demosite.local.crt file.

## View Certificates

```cli
openssl x509 -text -noout -in demosite.local.crt
```

## Convert

### Convert PEM to DER

```cli
openssl x509 \
-in demosite.local.crt \
-outform der \
-out demosite.local.der
```

### Convert PEM to PKCS12

```cli
openssl pkcs12 \
-inkey demosite.local.key \
-in demosite.local.crt \
-export \
-out demosite.local.pfx
```

## Types of certificate related files

SSL has been around for long enough you'd think that there would be agreed upon container formats. And you're right, there are. Too many standards as it happens. In the end, all of these are different ways to encode [Abstract Syntax Notation 1 (ASN.1)](https://en.wikipedia.org/wiki/ASN.1) formatted data — which happens to be the format x509 certificates are defined in — in machine-readable ways.

* **.csr** - This is a Certificate Signing Request. Some applications can generate these for submission to certificate-authorities. The actual format is PKCS10 which is defined in [RFC 2986](https://www.rfc-editor.org/rfc/rfc2986). It includes some/all of the key details of the requested certificate such as subject, organization, state, whatnot, as well as the _public key_ of the certificate to get signed. These get signed by the CA and a certificate is returned. The returned certificate is the public _certificate_ (which includes the public key but not the private key), which itself can be in a couple of formats.
* **.pem** - Defined in RFC [1422](https://www.rfc-editor.org/rfc/rfc1422) (part of a series from [1421](https://www.rfc-editor.org/rfc/rfc1421) through [1424](https://www.rfc-editor.org/rfc/rfc1424)) this is a container format that may include just the public certificate (such as with Apache installs, and CA certificate files `/etc/ssl/certs`), or may include an entire certificate chain including public key, private key, and root certificates. Confusingly, it may also encode a CSR (e.g. as used [here](https://jamielinux.com/docs/openssl-certificate-authority/create-the-intermediate-pair.html)) as the PKCS10 format can be translated into PEM. The name is from [Privacy Enhanced Mail (PEM)](https://en.wikipedia.org/wiki/Privacy-enhanced_Electronic_Mail), a failed method for secure email but the container format it used lives on, and is a base64 translation of the x509 ASN.1 keys.
* **.key** - This is a (usually) PEM formatted file containing just the private-key of a specific certificate and is merely a conventional name and not a standardized one. In Apache installs, this frequently resides in `/etc/ssl/private`. The rights on these files are very important, and some programs will refuse to load these certificates if they are set wrong.
* **.pkcs12 .pfx .p12** - Originally defined by RSA in the [Public-Key Cryptography Standards](https://en.wikipedia.org/wiki/PKCS) (abbreviated PKCS), the "12" variant was originally enhanced by Microsoft, and later submitted as [RFC 7292](https://www.rfc-editor.org/rfc/rfc7292). This is a password-protected container format that contains both public and private certificate pairs. Unlike .pem files, this container is fully encrypted. Openssl can turn this into a .pem file with both public and private keys: `openssl pkcs12 -in file-to-convert.p12 -out converted-file.pem -nodes`

A few other formats that show up from time to time:

* **.der** - A way to encode ASN.1 syntax in binary, a .pem file is just a Base64 encoded .der file. OpenSSL can convert these to .pem (`openssl x509 -inform der -in to-convert.der -out converted.pem`). Windows sees these as Certificate files. By default, Windows will export certificates as .DER formatted files with a different extension. Like...
* **.cert .cer .crt** - A .pem (or rarely .der) formatted file with a different extension, one that is recognized by Windows Explorer as a certificate, which .pem is not.
* **.p7b .keystore** - Defined in [RFC 2315](https://www.rfc-editor.org/rfc/rfc2315) as PKCS number 7, this is a format used by Windows for certificate interchange. Java understands these natively, and often uses `.keystore` as an extension instead. Unlike .pem style certificates, this format has a _defined_ way to include certification-path certificates.
* **.crl** - A certificate revocation list. Certificate Authorities produce these as a way to de-authorize certificates before expiration. You can sometimes download them from CA websites.

* * *

In summary, there are four different ways to present certificates and their components:

* **PEM** - Governed by RFCs, used preferentially by open-source software because it is text-based and therefore less prone to translation/transmission errors. It can have a variety of extensions (.pem, .key, .cer, .cert, more)
* **PKCS7** - An open standard used by Java and supported by Windows. Does not contain private key material.
* **PKCS12** - A Microsoft private standard that was later defined in an RFC that provides enhanced security versus the plain-text PEM format. This can contain private key and certificate chain material. Its used preferentially by Windows systems, and can be freely converted to PEM format through use of openssl.
* **DER** - The parent format of PEM. It's useful to think of it as a binary version of the base64-encoded PEM file. Not routinely used very much outside of Windows.


## Sources

* [Generating certificates](<https://www.baeldung.com/openssl-self-signed-cert>)
* [Certificate file types](<https://serverfault.com/questions/9708/what-is-a-pem-file-and-how-does-it-differ-from-other-openssl-generated-key-file/9717#9717>)
