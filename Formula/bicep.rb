# to get the SHA256 checksum, replace the below command with the appropriate version:
# curl -Lf https://github.com/Azure/bicep/releases/download/v0.2.212/bicep-osx-x64 | shasum -a 256

class Bicep < Formula
  desc "Bicep: next generation template language for Azure Resource Manager (ARM)"
  homepage "https://github.com/Azure/bicep"
  version "0.2.212"
  url "https://github.com/Azure/bicep/releases/download/v0.2.212/bicep-osx-x64"
  sha256 "fe47919059bf1334c1e3453e4f760fe1b41ca04b99d219acd3c34f4cca522dd7"

  license "MIT"

  def install
    bin.install "bicep-osx-x64" => "bicep"
  end

  test do
  end
end
