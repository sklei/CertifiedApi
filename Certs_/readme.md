# Steps

## Create Private Key

```cli
openssl genrsa -des3 -out demosite.local.key 2048

pass: welkom-1
```

This creates a .key file

## Create CSR

### With pre-made Private Key

```cli
openssl req -new \
-key demosite.local.key \
-out demosite.local.csr \
-config config.cnf
```

### Create and make new Private Key

```cli
openssl req -new \
-newkey rsa:2048 -nodes \
-keyout demosite.local.key \
-out demosite.local.csr \
-config config.cnf
```

## Creating a Self-Signed Certificate

### With premade CSR and KEY

```cli
openssl x509 \
-signkey demosite.local.key \
-in demosite.local.csr \
-req \
-days 365 \
-out demosite.local.crt
```

### Temporary CSR

```cli
openssl req \
-newkey rsa:2048 \
-keyout domain.key \
-x509 \
-days 365 \
-config config.cnf \
-out domain.crt
```

## Creating a CA-Signed Certificate With Our Own CA

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

Create a configuration text-file (domain.ext) with the following content:

```txt
authorityKeyIdentifier=keyid,issuer
basicConstraints=CA:FALSE
subjectAltName = @alt_names
[alt_names]
DNS.1 = domain
```

The “DNS.1” field should be the domain of our website.

Then we can sign our CSR (domain.csr) with the root CA certificate and its private key:

```cli
openssl x509 \
-req \
-CA rootCA.crt \
-CAkey rootCA.key \
-in domain.csr \
-out domain.crt \
-days 365 \
-CAcreateserial \
-extfile domain.ext
```

As a result, the CA-signed certificate will be in the domain.crt file.

## View Certificates

```cli
openssl x509 -text -noout -in domain.crt
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
