# Identity Service Secrets

Place the RSA or ECDSA key pair used to sign JWT tokens in this directory.

Expected files:

- `private.key`: PEM encoded private key (PKCS#8 or PKCS#1)
- `public.key`: PEM encoded public key

These files are mounted read-only into the identity API container at `/app/identity-secrets`.
